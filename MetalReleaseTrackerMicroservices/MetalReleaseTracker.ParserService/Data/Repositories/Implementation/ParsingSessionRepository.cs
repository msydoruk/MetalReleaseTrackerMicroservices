using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Entities;
using MetalReleaseTracker.ParserService.Data.Entities.Enums;
using MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MetalReleaseTracker.ParserService.Data.Repositories.Implementation;

public class ParsingSessionRepository : IParsingSessionRepository
{
    private readonly ParserServiceDbContext _context;
    private readonly ILogger<ParsingSessionRepository> _logger;

    public ParsingSessionRepository(ParserServiceDbContext context, ILogger<ParsingSessionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ParsingSessionEntity?> GetIncompleteAsync(DistributorCode distributorCode, CancellationToken cancellationToken)
    {
        var parsingState = await _context.ParsingSessions.FirstOrDefaultAsync(
            state => state.DistributorCode == distributorCode && state.ParsingStatus == AlbumParsingStatus.Incomplete,
            cancellationToken);

        return parsingState;
    }

    public async Task<List<ParsingSessionEntity>> GetParsedAsync(CancellationToken cancellationToken)
    {
        return await _context.ParsingSessions
            .Where(state => state.ParsingStatus == AlbumParsingStatus.Parsed)
            .ToListAsync(cancellationToken);
    }

    public async Task<ParsingSessionEntity> AddAsync(DistributorCode distributorCode, string nextPageToProcess, CancellationToken cancellationToken)
    {
        var parsingStateEntity = new ParsingSessionEntity
        {
            Id = Guid.NewGuid(),
            DistributorCode = distributorCode,
            PageToProcess = nextPageToProcess,
            LastUpdatedDate = DateTime.UtcNow
        };

        await _context.ParsingSessions.AddAsync(parsingStateEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return parsingStateEntity;
    }

    public async Task<bool> UpdateNextPageToProcessAsync(Guid id, string nextPageToProcess, CancellationToken cancellationToken)
    {
        var parsingState = await _context.ParsingSessions.FirstOrDefaultAsync(state => state.Id == id, cancellationToken);

        if (parsingState == null)
        {
            return false;
        }

        parsingState.PageToProcess = nextPageToProcess;
        parsingState.LastUpdatedDate = DateTime.UtcNow;

        _context.ParsingSessions.Update(parsingState);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateParsingStatus(Guid id, AlbumParsingStatus parsingStatus, CancellationToken cancellationToken)
    {
        var parsingState = await _context.ParsingSessions.FirstOrDefaultAsync(state => state.Id == id, cancellationToken);

        if (parsingState == null)
        {
            return false;
        }

        parsingState.ParsingStatus = parsingStatus;
        parsingState.LastUpdatedDate = DateTime.UtcNow;

        _context.ParsingSessions.Update(parsingState);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}