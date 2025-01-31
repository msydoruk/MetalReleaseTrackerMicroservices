namespace MetalReleaseTracker.ParserService.Parsers.Models;

public class PageParsedResult
{
    public IEnumerable<AlbumParsedEvent> ParsedAlbums { get; set; }

    public string NextPageUrl { get; set; }

    public bool HasMorePages => !string.IsNullOrEmpty(NextPageUrl);
}