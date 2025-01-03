using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MongoDB.Driver;

namespace MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;

public interface IRawAlbumRepository
{
    Task<List<RawAlbumEntity>> GetBatchByParsingSessionIdAsync(Guid parsingSessionId);

    Task AddAsync(List<RawAlbumEntity> rawAlbumEntities);
}