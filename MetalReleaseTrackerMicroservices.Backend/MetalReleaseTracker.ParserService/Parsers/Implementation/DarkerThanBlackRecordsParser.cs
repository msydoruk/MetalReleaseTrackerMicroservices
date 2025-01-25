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

    public async Task<PageParsedResult> ParseAsync(string parsingUrl, CancellationToken cancellationToken)
    {
        return new PageParsedResult();
    }
}