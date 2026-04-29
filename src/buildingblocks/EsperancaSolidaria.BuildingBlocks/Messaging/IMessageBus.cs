namespace EsperancaSolidaria.BuildingBlocks.Messaging;

public interface IMessageBus : IAsyncDisposable
{
    Task PublishAsync(object message, string queueName, CancellationToken cancellationToken = default);
    Task ConsumeAsync<T>(
        string queueName,
        Func<T, CancellationToken, Task<bool>> handler,
        CancellationToken cancellationToken = default);
}