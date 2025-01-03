using MetalReleaseTracker.CatalogSyncService.Configurations;
using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MetalReleaseTracker.CatalogSyncService.Data.Repositories.Implementation;

public class ParsingSessionRepository : IParsingSessionRepository
{
    private readonly IMongoCollection<ParsingSessionEntity> _parsingSessionCollection;

    public ParsingSessionRepository(IMongoDatabase database, IOptions<MongoDbConfig> mongoDbConfig)
    {
        _parsingSessionCollection = database.GetCollection<ParsingSessionEntity>(mongoDbConfig.Value.ParsingSessionCollectionName);
    }

    public async Task<List<ParsingSessionEntity>> GetUnProcessedAsync()
    {
        var filter = Builders<ParsingSessionEntity>.Filter
            .In(album => album.ProcessingStatus, new[]
            {
                ParsingSessionProcessingStatus.Commited,
                ParsingSessionProcessingStatus.Failed
            });
        var order = Builders<ParsingSessionEntity>.Sort.Ascending(album => album.CreatedDate);

        return await _parsingSessionCollection.Find(filter).Sort(order).ToListAsync();
    }

    public async Task AddAsync(ParsingSessionEntity parsingSessionEntity)
    {
        await _parsingSessionCollection.InsertOneAsync(parsingSessionEntity);
    }

    public async Task UpdateProcessingStatusAsync(Guid id, ParsingSessionProcessingStatus processingStatus)
    {
        var filter = Builders<ParsingSessionEntity>.Filter.Eq(album => album.Id, id);

        var updates = new List<UpdateDefinition<ParsingSessionEntity>>
        {
            Builders<ParsingSessionEntity>.Update.Set(album => album.ProcessingStatus, processingStatus),
            Builders<ParsingSessionEntity>.Update.Set(album => album.LastUpdateDate, DateTime.UtcNow)
        };

        if (processingStatus == ParsingSessionProcessingStatus.Processed)
        {
            updates.Add(Builders<ParsingSessionEntity>.Update.Set(album => album.ProcessedDate, DateTime.UtcNow));
        }

        var updateDefinition = Builders<ParsingSessionEntity>.Update.Combine(updates);

        await _parsingSessionCollection.UpdateOneAsync(filter, updateDefinition);
    }
}