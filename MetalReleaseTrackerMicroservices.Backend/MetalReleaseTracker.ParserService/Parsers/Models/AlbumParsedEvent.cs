using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MetalReleaseTracker.ParserService.Parsers.Models;

public class AlbumParsedEvent : AlbumBase
{
    public DateTime CreatedDate { get; set; }
}