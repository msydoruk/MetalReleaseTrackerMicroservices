using HtmlAgilityPack;

namespace MetalReleaseTracker.ParserService.Helpers;

public interface IHtmlDocumentLoader
{
    Task<HtmlDocument> LoadHtmlDocumentAsync(string url, CancellationToken cancellationToken);
}