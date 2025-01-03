using Flurl.Http;
using HtmlAgilityPack;

namespace MetalReleaseTracker.ParserService.Helpers
{
    public class HtmlDocumentLoader : IHtmlDocumentLoader
    {
        public async Task<HtmlDocument> LoadHtmlDocumentAsync(string url, CancellationToken cancellationToken)
        {
            var pageContent = await url.WithTimeout(TimeSpan.FromSeconds(30))
                .GetStringAsync(cancellationToken: cancellationToken);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(pageContent);

            return htmlDocument;
        }
    }
}