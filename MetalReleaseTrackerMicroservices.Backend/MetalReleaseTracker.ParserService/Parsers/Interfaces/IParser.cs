using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.Parsers.Interfaces;

public interface IParser
{
    DistributorCode DistributorCode { get; }

    Task<PageParsedResult> ParseAsync(string parsingUrl, CancellationToken cancellationToken);
}