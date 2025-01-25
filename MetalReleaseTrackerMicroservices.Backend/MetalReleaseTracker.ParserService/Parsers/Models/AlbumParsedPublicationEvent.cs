using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MetalReleaseTracker.ParserService.Parsers.Models;

public class AlbumParsedPublicationEvent
{
    public DateTime CreatedDate { get; set; }

    public Guid ParsingSessionId { get; set; }

    public DistributorCode DistributorCode { get; set; }

    public List<string> StorageFilePaths { get; set; }
}