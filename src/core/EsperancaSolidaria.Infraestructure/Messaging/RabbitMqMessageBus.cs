using System.Text;
using System.Text.Json;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EsperancaSolidaria.Infraestructure.Messaging;

public class RabbitMqMessageBus : IMessageBus, IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessageBus> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    // Retry Policy
    private int _retryCount = 3;
    private int _retryIntervalMilliseconds = 10000; // 10 segundos

    // Naming Convention
    private string _exchangeSuffix => "_exchange";
    private string _queueSuffix => "_queue";
    private string _retrySuffix => "_retry";
    private string _dlqSuffix => "_dlq";
    private string _trashSuffix => "_trash";
    private string _messageRoutingKeySuffix => ".message";
    private string _retryRoutingKeySuffix => ".retry";

    public RabbitMqMessageBus(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqMessageBus> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    #region Connection & Channel Management

    private async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection != null && _connection.IsOpen)
        {
            _logger.LogDebug("Usando conexão RabbitMQ existente");
            return _connection;
        }

        _logger.LogInformation("Criando nova conexão RabbitMQ para {HostName}:{Port}",
            _options.HostName, _options.Port);

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
            SocketReadTimeout = TimeSpan.FromSeconds(30),
            SocketWriteTimeout = TimeSpan.FromSeconds(30),
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        try
        {
            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _logger.LogInformation("Conexão RabbitMQ estabelecida com sucesso");
            return _connection;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao criar conexão com RabbitMQ");
            throw;
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_channel != null && _channel.IsOpen)
        {
            _logger.LogDebug("Usando canal RabbitMQ existente");
            return _channel;
        }

        _logger.LogDebug("Criando novo canal RabbitMQ");

        var connection = await GetConnectionAsync(cancellationToken);
        _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        return _channel;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Publica uma mensagem na fila principal
    /// </summary>
    public async Task PublishAsync(
        object message,
        string queueName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);

        var channel = await GetChannelAsync(cancellationToken);
        var exchange = GetExchangeName(queueName);
        var routingKey = GetRoutingKey(queueName, RoutingKeyType.Message);

        var body = SerializeMessage(message);

        // Configurar propriedades durável
        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Persistent = true,
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        try
        {
            await channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Mensagem publicada com sucesso na fila {QueueName} - Routing Key: {RoutingKey}",
                queueName, routingKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem na fila {QueueName}", queueName);
            throw;
        }
    }

    /// <summary>
    /// Consome mensagens de uma fila com tipo seguro
    /// </summary>
    public async Task ConsumeAsync<T>(
        string queueName,
        Func<T, CancellationToken, Task<bool>> handler,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(queueName);
        ArgumentNullException.ThrowIfNull(handler);

        // Inicializa as filas antes de começar a consumir
        await InitializeQueuesAsync(queueName, cancellationToken);

        var channel = await GetChannelAsync(cancellationToken);
        var mainQueueName = GetQueueName(queueName, QueueType.Main);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            try
            {
                var messageJson = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var retryCount = GetRetryCountFromHeaders(eventArgs.BasicProperties?.Headers);

                _logger.LogDebug("Mensagem recebida da fila {QueueName} - Tentativa {RetryCount}",
                    queueName, retryCount + 1);

                // Tentar desserializar
                T? deserializedMessage;
                try
                {
                    deserializedMessage = JsonSerializer.Deserialize<T>(messageJson);

                    if (deserializedMessage == null)
                    {
                        throw new JsonException("Desserialização resultou em null");
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Falha ao desserializar mensagem para tipo {Type} na fila {QueueName}",
                        typeof(T).Name, queueName);

                    // Enviar para trash queue
                    await SendToTrashQueueAsync(channel, queueName, messageJson, ex, cancellationToken);
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                    return;
                }

                // Executar handler com política de retry
                bool success = false;
                Exception? lastException = null;

                try
                {
                    var result = await handler(deserializedMessage, cancellationToken);
                    success = result;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning(ex, $"Erro ao processar mensagem da fila {queueName} - Tentativa {_retryCount}");
                    success = false;
                }

                // Decisão baseada no resultado
                if (success)
                {
                    // ACK bem-sucedido
                    await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                    _logger.LogInformation("Mensagem processada com sucesso da fila {QueueName}", queueName);
                }
                else
                {
                    // Verificar se já tentou max vezes
                    if (retryCount >= _retryCount)
                    {
                        // Enviar para DLQ
                        await SendToDlqAsync(
                            channel,
                            queueName,
                            messageJson,
                            "Esgotadas as tentativas de retry",
                            retryCount + 1,
                            lastException,
                            cancellationToken);

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                    }
                    else
                    {
                        // Enviar para retry queue
                        await SendToRetryQueueAsync(
                            channel,
                            queueName,
                            messageJson,
                            retryCount + 1,
                            cancellationToken);

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao processar mensagem da fila {QueueName}", queueName);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, cancellationToken);
            }
        };

        try
        {
            await channel.BasicConsumeAsync(
                queue: mainQueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Iniciado consumo de mensagens da fila {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar consumo da fila {QueueName}", queueName);
            throw;
        }
    }

    #endregion

    #region Private Methods - Queue Initialization

    /// <summary>
    /// Inicializa as 4 filas: Main, Retry, DLQ e Trash
    /// </summary>
    private async Task InitializeQueuesAsync(string queueName, CancellationToken cancellationToken = default)
    {
        var channel = await GetChannelAsync(cancellationToken);
        var exchange = GetExchangeName(queueName);

        try
        {
            // Declarar exchange
            await channel.ExchangeDeclareAsync(
                exchange: exchange,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            // 1. FILA PRINCIPAL (Main Queue)
            await DeclareMainQueueAsync(channel, queueName, exchange, cancellationToken);

            // 2. FILA DE RETRY
            await DeclareRetryQueueAsync(channel, queueName, exchange, cancellationToken);

            // 3. FILA DE DLQ (Dead Letter Queue)
            await DeclareDlqAsync(channel, queueName, exchange, cancellationToken);

            // 4. FILA DE TRASH (mensagens inválidas)
            await DeclareTrashQueueAsync(channel, queueName, cancellationToken);

            _logger.LogInformation("Filas inicializadas com sucesso para {QueueName}", queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar filas para {QueueName}", queueName);
            throw;
        }
    }

    private async Task DeclareMainQueueAsync(
        IChannel channel,
        string queueName,
        string exchange,
        CancellationToken cancellationToken)
    {
        var queueNameMain = GetQueueName(queueName, QueueType.Main);
        var routingKeyMain = GetRoutingKey(queueName, RoutingKeyType.Message);

        var arguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", exchange },
            { "x-dead-letter-routing-key", GetRoutingKey(queueName, RoutingKeyType.Retry) }
        };

        await channel.QueueDeclareAsync(
            queue: queueNameMain,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: queueNameMain,
            exchange: exchange,
            routingKey: routingKeyMain,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Fila principal criada: {QueueName}", queueNameMain);
    }

    private async Task DeclareRetryQueueAsync(
        IChannel channel,
        string queueName,
        string exchange,
        CancellationToken cancellationToken)
    {
        var queueNameRetry = GetQueueName(queueName, QueueType.Retry);
        var routingKeyRetry = GetRoutingKey(queueName, RoutingKeyType.Retry);

        var arguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", exchange },
            { "x-dead-letter-routing-key", GetRoutingKey(queueName, RoutingKeyType.Message) },
            { "x-message-ttl", _retryIntervalMilliseconds }
        };

        await channel.QueueDeclareAsync(
            queue: queueNameRetry,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: arguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: queueNameRetry,
            exchange: exchange,
            routingKey: routingKeyRetry,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Fila de retry criada: {QueueName}", queueNameRetry);
    }

    private async Task DeclareDlqAsync(
        IChannel channel,
        string queueName,
        string exchange,
        CancellationToken cancellationToken)
    {
        var queueNameDlq = GetQueueName(queueName, QueueType.Dlq);

        await channel.QueueDeclareAsync(
            queue: queueNameDlq,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Fila DLQ criada: {QueueName}", queueNameDlq);
    }

    private async Task DeclareTrashQueueAsync(
        IChannel channel,
        string queueName,
        CancellationToken cancellationToken)
    {
        var queueNameTrash = GetQueueName(queueName, QueueType.Trash);

        await channel.QueueDeclareAsync(
            queue: queueNameTrash,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        _logger.LogDebug("Fila Trash criada: {QueueName}", queueNameTrash);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Envia mensagem para a fila de DLQ quando esgota as tentativas de retry
    /// </summary>
    private async Task SendToDlqAsync(
        IChannel channel,
        string queueName,
        string messageJson,
        string reason,
        int retryAttempts,
        Exception? exception = null,
        CancellationToken cancellationToken = default)
    {
        var dlqQueueName = GetQueueName(queueName, QueueType.Dlq);
        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                { "x-failure-reason", reason },
                { "x-retry-attempts", retryAttempts },
                { "x-original-queue", queueName },
                { "x-failed-at", DateTime.UtcNow.ToString("O") }
            }
        };

        if (exception != null)
        {
            properties.Headers["x-error-message"] = exception.Message;
            properties.Headers["x-error-stacktrace"] = exception.StackTrace;
        }

        try
        {
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: dlqQueueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogError(
                "Mensagem enviada para DLQ {DlqQueue} após {RetryAttempts} tentativas. Razão: {Reason}",
                dlqQueueName, retryAttempts, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para DLQ {DlqQueue}", dlqQueueName);
        }
    }

    /// <summary>
    /// Envia mensagem para a fila de retry
    /// </summary>
    private async Task SendToRetryQueueAsync(
        IChannel channel,
        string queueName,
        string messageJson,
        int currentAttempt,
        CancellationToken cancellationToken = default)
    {
        var retryQueueName = GetQueueName(queueName, QueueType.Retry);
        var routingKeyRetry = GetRoutingKey(queueName, RoutingKeyType.Retry);
        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Persistent = true,
            ContentType = "application/json",
            ContentEncoding = "utf-8",
            Headers = new Dictionary<string, object?>
            {
                { "x-retry-count", currentAttempt },
                { "x-original-queue", queueName },
                { "x-sent-to-retry-at", DateTime.UtcNow.ToString("O") }
            }
        };

        try
        {
            await channel.BasicPublishAsync(
                exchange: GetExchangeName(queueName),
                routingKey: routingKeyRetry,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogWarning(
                "Mensagem enviada para fila de retry {RetryQueue} - Tentativa {Attempt} de {MaxRetries}",
                retryQueueName, currentAttempt, _retryCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para retry queue {RetryQueue}", retryQueueName);
        }
    }

    /// <summary>
    /// Envia mensagem para a fila de trash quando não consegue desserializar
    /// </summary>
    private async Task SendToTrashQueueAsync(
        IChannel channel,
        string queueName,
        string messageJson,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var trashQueueName = GetQueueName(queueName, QueueType.Trash);
        var body = Encoding.UTF8.GetBytes(messageJson);

        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                { "x-error-message", exception.Message },
                { "x-error-stacktrace", exception.StackTrace },
                { "x-original-queue", queueName }
            }
        };

        try
        {
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: trashQueueName,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogWarning(
                "Mensagem enviada para trash queue {TrashQueue}. Erro: {Error}",
                trashQueueName, exception.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para trash queue {TrashQueue}", trashQueueName);
        }
    }

    private byte[] SerializeMessage(object message)
    {
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Extrai o contador de tentativas de retry dos headers da mensagem
    /// </summary>
    private int GetRetryCountFromHeaders(IDictionary<string, object?>? headers)
    {
        if (headers == null || !headers.ContainsKey("x-retry-count"))
            return 0;

        var retryCountObj = headers["x-retry-count"];
        if (retryCountObj is int retryCount)
            return retryCount;

        if (retryCountObj is byte[] byteArray && int.TryParse(Encoding.UTF8.GetString(byteArray), out var parsedCount))
            return parsedCount;

        return 0;
    }

    private string GetExchangeName(string queueName) =>
        $"{queueName}{_exchangeSuffix}";

    private string GetQueueName(string queueName, QueueType queueType) =>
        queueType switch
        {
            QueueType.Main => $"{queueName}{_queueSuffix}",
            QueueType.Retry => $"{queueName}{_retrySuffix}",
            QueueType.Dlq => $"{queueName}{_dlqSuffix}",
            QueueType.Trash => $"{queueName}{_trashSuffix}",
            _ => throw new ArgumentOutOfRangeException(nameof(queueType))
        };

    private string GetRoutingKey(string queueName, RoutingKeyType routingKeyType) =>
        routingKeyType switch
        {
            RoutingKeyType.Message => $"{queueName}{_messageRoutingKeySuffix}",
            RoutingKeyType.Retry => $"{queueName}{_retryRoutingKeySuffix}",
            _ => throw new ArgumentOutOfRangeException(nameof(routingKeyType))
        };

    #endregion

    #region Enums

    private enum QueueType
    {
        Main,
        Retry,
        Dlq,
        Trash
    }

    private enum RoutingKeyType
    {
        Message,
        Retry
    }

    #endregion

    #region Dispose

    public async ValueTask DisposeAsync()
    {
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
            _logger.LogInformation("Canal RabbitMQ fechado");
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
            _logger.LogInformation("Conexão RabbitMQ fechada");
        }

        GC.SuppressFinalize(this);
    }

    #endregion
}