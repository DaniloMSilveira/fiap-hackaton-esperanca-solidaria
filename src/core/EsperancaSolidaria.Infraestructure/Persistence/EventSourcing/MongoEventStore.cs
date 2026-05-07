using EsperancaSolidaria.BuildingBlocks.EventSourcing;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EsperancaSolidaria.Infraestructure.Persistence.EventSourcing;

/// <summary>
/// Implementação de Event Store usando MongoDB para persistência de eventos
/// </summary>
public class MongoEventStore : IEventStore
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<EventStoreDocument> _eventsCollection;
    private readonly ILogger<MongoEventStore> _logger;

    public MongoEventStore(IMongoDatabase database, ILogger<MongoEventStore> logger)
    {
        _database = database;
        _logger = logger;
        
        _eventsCollection = _database.GetCollection<EventStoreDocument>("events");
    }

    public async Task SaveEventAsync(
        string eventName,
        Guid eventId,
        object eventData,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bsonData = eventData.ToBsonDocument();
            
            var eventDocument = new EventStoreDocument
            {
                EventId = eventId,
                EventName = eventName,
                Data = bsonData,
                CorrelationId = correlationId,
                Timestamp = DateTime.Now
            };

            await _eventsCollection.InsertOneAsync(
                eventDocument,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Evento {EventName} com ID {EventId} persistido com sucesso no MongoDB",
                eventName, eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao persistir evento {EventName} no MongoDB", eventName);
            throw;
        }
    }
}