using MassTransit;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using MetalReleaseTracker.ParserService.Parsers.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.Parsers.Implementation;

public class DarkerThanBlackRecordsParser : IParser
{
    private readonly ILogger<DarkerThanBlackRecordsParser> _logger;

    public DistributorCode DistributorCode => DistributorCode.DarkThanBlackRecords;

    public DarkerThanBlackRecordsParser(ILogger<DarkerThanBlackRecordsParser> logger)
    {
        _logger = logger;
    }

    public async IAsyncEnumerable<AlbumParsedEvent> ParseAsync(string parsingUrl, CancellationToken cancellationToken)
    {
        yield return new AlbumParsedEvent
        {
            DistributorCode = DistributorCode,
            Name = "Test album Darker Than Black Records"
        };
    }
}