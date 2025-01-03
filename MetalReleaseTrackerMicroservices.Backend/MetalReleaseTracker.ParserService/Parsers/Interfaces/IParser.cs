using System.Runtime.CompilerServices;
using MassTransit;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.Parsers.Interfaces;

public interface IParser
{
    DistributorCode DistributorCode { get; }

    IAsyncEnumerable<AlbumParsedEvent> ParseAsync(string parsingUrl, CancellationToken cancellationToken);
}