using MetalReleaseTracker.CatalogSyncService.Configurations;
using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MetalReleaseTracker.CatalogSyncService.Data.Repositories.Implementation;

public class RawAlbumRepository : IRawAlbumRepository
{
    private readonly IMongoCollection<RawAlbumEntity> _rawAlbumsCollection;

    public RawAlbumRepository(IMongoDatabase database, IOptions<MongoDbConfig> mongoDbConfig)
    {
        _rawAlbumsCollection = database.GetCollection<RawAlbumEntity>(mongoDbConfig.Value.RawAlbumsCollectionName);
    }

    public async Task<List<RawAlbumEntity>> GetBatchByParsingSessionIdAsync(Guid parsingSessionId)
    {
        var filter = Builders<RawAlbumEntity>.Filter.Eq(album => album.ParsingSessionId, parsingSessionId);

        return await _rawAlbumsCollection.Find(filter).ToListAsync();
    }

    public async Task AddAsync(List<RawAlbumEntity> rawAlbumEntities)
    {
        await _rawAlbumsCollection.InsertManyAsync(rawAlbumEntities);
    }
}