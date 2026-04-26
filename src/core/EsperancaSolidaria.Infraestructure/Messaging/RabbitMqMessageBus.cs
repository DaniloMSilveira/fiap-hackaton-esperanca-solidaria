using System.Text;
using System.Text.Json;
using EsperancaSolidaria.BuildingBlocks.Messaging;
using EsperancaSolidaria.Infraestructure.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMqMessageBus : IMessageBus, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqMessageBus> _logger;
    private readonly Lazy<Task<IConnection>> _connection;
    private bool _isDisposed;

    private IChannel? _publishChannel;
    private readonly IAsyncPolicy<bool> _publishPolicy;

    public RabbitMqMessageBus(IOptions<RabbitMqOptions> options, ILogger<RabbitMqMessageBus> logger)
    {
        _options = options.Value;
        _logger = logger;
        _connection = new Lazy<Task<IConnection>>(CreateConnectionAsync);

        _publishPolicy = Policy
            .Handle<Exception>()
            .OrResult<bool>(r => !r)
            .CircuitBreakerAsync<bool>(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (outcome, duration) => _logger.LogError("Circuit breaker aberto"),
                onReset: () => _logger.LogInformation("Circuit breaker fechado"));
    }

    private async Task<IConnection> CreateConnectionAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true
        };
        return await factory.CreateConnectionAsync();
    }

    private async Task<IChannel> GetPublishChannelAsync(CancellationToken cancellationToken = default)
    {
        if (_publishChannel?.IsOpen == true) return _publishChannel;
        var connection = await _connection.Value;
        _publishChannel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        return _publishChannel;
    }

    public async Task InitializeQueuesAsync(CancellationToken cancellationToken = default)
    {
        var connection = await _connection.Value;
        using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var exchange = $"{_options.QueueName}_exchange";
        await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true);

        // Fila principal
        try
        {
            await channel.QueueDeclarePassiveAsync($"{_options.QueueName}_queue", cancellationToken);
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            await channel.QueueDeclareAsync(
                queue: $"{_options.QueueName}_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: $"{_options.QueueName}_queue",
                exchange: exchange,
                routingKey: $"{_options.QueueName}.message",
                cancellationToken: cancellationToken);
        }

        // Fila de retry
        try
        {
            await channel.QueueDeclarePassiveAsync($"{_options.QueueName}_retry", cancellationToken);
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            await channel.QueueDeclareAsync(
                queue: $"{_options.QueueName}_retry",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", exchange },
                    { "x-dead-letter-routing-key", $"{_options.QueueName}.message" },
                    { "x-message-ttl", 10000 }
                },
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: $"{_options.QueueName}_retry",
                exchange: exchange,
                routingKey: $"{_options.QueueName}.retry",
                cancellationToken: cancellationToken);
        }
    }

    public async Task PublishAsync(object message, string queueName, CancellationToken cancellationToken = default)
    {
        var exchange = $"{queueName}_exchange";
        var routingKey = $"{queueName}.message";

        var result = await _publishPolicy.ExecuteAsync(async () =>
        {
            var channel = await GetPublishChannelAsync(cancellationToken);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(exchange, routingKey, body: body, cancellationToken: cancellationToken);
            return true;
        });

        if (!result) throw new InvalidOperationException("Circuit breaker aberto");
    }

    public async Task ConsumeAsync(
        string queueName,
        Func<string, Task> handler,
        CancellationToken cancellationToken = default)
    {
        var connection = await _connection.Value;
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);

            try
            {
                await handler(messageJson);
                await channel.BasicAckAsync(eventArgs.DeliveryTag, false, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, cancellationToken);
            }
        };

        await channel.BasicConsumeAsync(queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;

        _publishChannel?.Dispose();
        if (_connection.IsValueCreated) _connection.Value?.Dispose();
    }
}