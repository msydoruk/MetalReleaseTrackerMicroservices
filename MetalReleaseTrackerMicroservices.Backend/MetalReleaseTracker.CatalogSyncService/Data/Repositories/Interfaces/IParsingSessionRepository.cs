using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MongoDB.Driver;

namespace MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;

public interface IParsingSessionRepository
{
    Task<List<ParsingSessionEntity>> GetUnProcessedAsync();

    Task AddAsync(ParsingSessionEntity parsingSessionEntity);

    Task UpdateProcessingStatusAsync(Guid id, ParsingSessionProcessingStatus processingStatus);
}