using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Entities;
using MetalReleaseTracker.ParserService.Data.Entities.Enums;

namespace MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;

public interface IParsingSessionRepository
{
    Task<ParsingSessionEntity?> GetIncompleteAsync(DistributorCode distributorCode, CancellationToken cancellationToken);

    Task<List<ParsingSessionEntity>> GetParsedAsync(CancellationToken cancellationToken);

    Task<ParsingSessionEntity> AddAsync(DistributorCode distributorCode, string nextPageToProcess, CancellationToken cancellationToken);

    Task<bool> UpdateNextPageToProcessAsync(Guid parsingSessionId, string nextPageToProcess, CancellationToken cancellationToken);

    Task<bool> UpdateParsingStatus(Guid parsingSessionId, AlbumParsingStatus parsingStatus, CancellationToken cancellationToken);
}