namespace EsperancaSolidaria.BuildingBlocks.Messaging;

public interface IMessageBus : IDisposable
{
    Task InitializeQueuesAsync(CancellationToken cancellationToken = default);
    Task PublishAsync(object message, string queueName, CancellationToken cancellationToken = default);
    Task ConsumeAsync(string queueName, Func<string, Task> handler, CancellationToken cancellationToken = default);
}