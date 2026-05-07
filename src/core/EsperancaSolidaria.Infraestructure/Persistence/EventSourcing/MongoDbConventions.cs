using EsperancaSolidaria.BuildingBlocks.EventSourcing;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace EsperancaSolidaria.Infraestructure.Persistence.EventSourcing;

/// <summary>
/// Configuração de conventions do MongoDB para Event Sourcing
/// </summary>
public static class MongoDbConventions
{
    public static void Configure()
    {
        // Configurar serialização global de Guid e Object para eventos
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new ObjectSerializer(x => true));

        if (!BsonClassMap.IsClassMapRegistered(typeof(EventStoreDocument)))
        {
            BsonClassMap.RegisterClassMap<EventStoreDocument>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.MapIdMember(c => c.EventId);
                cm.MapMember(c => c.EventId).SetElementName("_id");
                cm.MapMember(c => c.EventName).SetElementName("eventName");
                cm.MapMember(c => c.Data).SetElementName("data").SetSerializer(new ObjectSerializer());
                cm.MapMember(c => c.CorrelationId).SetElementName("correlationId");
                cm.MapMember(c => c.Timestamp).SetElementName("timestamp");
            });
        }
    }
}
