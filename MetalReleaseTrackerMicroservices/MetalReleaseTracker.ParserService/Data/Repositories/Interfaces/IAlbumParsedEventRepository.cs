using MetalReleaseTracker.ParserService.Data.Entities;

namespace MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;

public interface IAlbumParsedEventRepository
{
    Task<List<AlbumParsedEventEntity>> GeAsync(Guid parsingSessionId, CancellationToken cancellationToken);

    Task AddAsync(Guid parsingSessionId, string eventPayload, CancellationToken cancellationToken);
}