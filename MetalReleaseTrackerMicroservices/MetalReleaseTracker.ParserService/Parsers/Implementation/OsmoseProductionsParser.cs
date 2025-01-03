using System.Runtime.CompilerServices;
using HtmlAgilityPack;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Entities.Enums;
using MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;
using MetalReleaseTracker.ParserService.Exceptions;
using MetalReleaseTracker.ParserService.Helpers;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using MetalReleaseTracker.ParserService.Parsers.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Models;
using Microsoft.Extensions.Options;

namespace MetalReleaseTracker.ParserService.Parsers.Implementation;

public class OsmoseProductionsParser : IParser
    {
        private readonly IHtmlDocumentLoader _htmlDocumentLoader;
        private readonly GeneralParserSettings _generalParserSettings;
        private readonly IParsingSessionRepository _parsingSessionRepository;
        private readonly ILogger<OsmoseProductionsParser> _logger;

        public DistributorCode DistributorCode => DistributorCode.OsmoseProductions;

        public OsmoseProductionsParser(
            IHtmlDocumentLoader htmlDocumentLoader,
            IOptions<GeneralParserSettings> generalParserSettings,
            IParsingSessionRepository parsingSessionRepository,
            ILogger<OsmoseProductionsParser> logger)
        {
            _htmlDocumentLoader = htmlDocumentLoader;
            _generalParserSettings = generalParserSettings.Value;
            _parsingSessionRepository = parsingSessionRepository;
            _logger = logger;
        }

        public async IAsyncEnumerable<AlbumParsedEvent> ParseAsync(string parsingUrl,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var parsingSession = await _parsingSessionRepository.GetIncompleteAsync(DistributorCode, cancellationToken);

            var nextPageUrl = parsingUrl;
            if (parsingSession == null)
            {
                parsingSession = await _parsingSessionRepository.AddAsync(DistributorCode, nextPageUrl, cancellationToken);
            }
            else
            {
                nextPageUrl = parsingSession.PageToProcess;
                _logger.LogInformation($"Restored parsing url {nextPageUrl} from state.");
            }

            _logger.LogInformation($"Starting album parsing from distributor URL: {nextPageUrl}.");

            bool hasMorePages = false;
            var totalParsedAlbumsCount = 0;
            do
            {
                _logger.LogInformation($"Parsing albums from URL: {nextPageUrl}.");
                var htmlDocument = await LoadHtmlDocument(nextPageUrl, cancellationToken);
                var albumUrls = ParseAlbumUrls(htmlDocument, cancellationToken);

                foreach (var albumUrl in albumUrls)
                {
                    AlbumParsedEvent albumParsedEvent = await ParseAlbumDetails(albumUrl, cancellationToken);
                    albumParsedEvent.ParsingSessionId = parsingSession.Id;

                    yield return albumParsedEvent;

                    totalParsedAlbumsCount++;
                }

                (nextPageUrl, hasMorePages) = GetNextPageUrl(htmlDocument);

                if (hasMorePages)
                {
                    await _parsingSessionRepository.UpdateNextPageToProcessAsync(parsingSession.Id, nextPageUrl, cancellationToken);
                }

                await Task.Delay(_generalParserSettings.PageDelayMilliseconds, cancellationToken);
            }
            while (hasMorePages);

            await _parsingSessionRepository.UpdateParsingStatus(parsingSession.Id, AlbumParsingStatus.Parsed, cancellationToken);

            _logger.LogInformation($"Completed album parsing from distributor URL: {parsingUrl}. Total albums parsed: {totalParsedAlbumsCount}.");
        }

        private List<string> ParseAlbumUrls(HtmlDocument htmlDocument, CancellationToken cancellationToken)
        {
            var albumUrls = new List<string>();
            var albumNodes = htmlDocument.DocumentNode.SelectNodes(".//div[@class='row GshopListingA']//div[@class='column three mobile-four']");

            if (albumNodes != null)
            {
                foreach (var albumNode in albumNodes)
                {
                    var albumUrl = albumNode.SelectSingleNode(".//a").GetAttributeValue("href", string.Empty).Trim();
                    albumUrls.Add(albumUrl);
                }
            }

            return albumUrls;
        }

        private async Task<AlbumParsedEvent> ParseAlbumDetails(string albumUrl, CancellationToken cancellationToken)
        {
            var htmlDocument = await LoadHtmlDocument(albumUrl, cancellationToken);

            var bandName = ParseBandName(htmlDocument);
            var sku = ParseSku(htmlDocument);
            var name = ParseAlbumName(htmlDocument);
            var releaseDate = ParseReleaseDate(htmlDocument);
            var price = ParsePrice(htmlDocument);
            var photoUrl = ParsePhotoUrl(htmlDocument);
            var media = ParseMediaType(htmlDocument);
            var label = ParseLabel(htmlDocument);
            var press = ParsePress(htmlDocument);
            var description = ParseDescription(htmlDocument);

            return new AlbumParsedEvent
            {
                DistributorCode = DistributorCode,
                BandName = bandName,
                SKU = sku,
                Name = name,
                ReleaseDate = releaseDate,
                Price = price,
                PurchaseUrl = albumUrl,
                PhotoUrl = photoUrl,
                Media = media,
                Label = label,
                Press = press,
                Description = description
            };
        }

        private (string nextPageUrl, bool hasMorePages) GetNextPageUrl(HtmlDocument htmlDocument)
        {
            var currentPageNode = htmlDocument.DocumentNode.SelectSingleNode(".//div[@class='GtoursPaginationButtonTxt on']/span");

            if (currentPageNode != null && int.TryParse(currentPageNode.InnerText.Trim(), out int currentPageNumber))
            {
                var nextPageNode = htmlDocument.DocumentNode.SelectSingleNode($".//a[contains(@href, 'page={currentPageNumber + 1}')]");

                if (nextPageNode != null)
                {
                    string nextPageUrl = nextPageNode.GetAttributeValue("href", null);
                    _logger.LogInformation($"Next page found: {nextPageUrl}.");

                    return (nextPageUrl, true);
                }
            }

            _logger.LogInformation("Next page not found.");
            return (null, false)!;
        }

        private string GetNodeValue(HtmlDocument document, string xPath)
        {
            var node = document.DocumentNode.SelectSingleNode(xPath);
            return node?.InnerText?.Trim();
        }

        private async Task<HtmlDocument> LoadHtmlDocument(string url, CancellationToken cancellationToken)
        {
            var htmlDocument = await _htmlDocumentLoader.LoadHtmlDocumentAsync(url, cancellationToken);

            if (htmlDocument?.DocumentNode == null)
            {
                var error = $"Failed to load or parse the HTML document {url}.";
                _logger.LogError(error);
                throw new OsmoseProductionsParserException(error);
            }

            return htmlDocument;
        }

        private string ParseBandName(HtmlDocument htmlDocument)
        {
            var bandName = GetNodeValue(htmlDocument, "//span[@class='cufonAb']/a");

            return bandName;
        }

        private string ParseAlbumName(HtmlDocument htmlDocument)
        {
            var nameNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='column twelve']//span[@class='cufonAb']");
            var nameHtml = nameNode?.InnerHtml;
            var name = !string.IsNullOrEmpty(nameHtml)
                ? nameHtml.Split(new[] { "</a>&nbsp;" }, StringSplitOptions.None).Last().Trim()
                : null;

            return name;
        }

        private string ParseSku(HtmlDocument htmlDocument)
        {
            return GetNodeValue(htmlDocument, "//span[@class='cufonEb' and contains(text(), 'Press :')]")?.Split(':').Last().Trim();
        }

        private DateTime ParseReleaseDate(HtmlDocument htmlDocument)
        {
            var releaseDateText = GetNodeValue(htmlDocument, "//span[@class='cufonEb' and contains(text(), 'Year :')]");
            return !string.IsNullOrEmpty(releaseDateText) ? AlbumParsingHelper.ParseYear(releaseDateText.Split(':').Last().Trim()) : DateTime.MinValue;
        }

        private float ParsePrice(HtmlDocument htmlDocument)
        {
            var priceText = GetNodeValue(htmlDocument, "//span[@class='cufonCd ']");
            priceText = priceText?.Replace("&nbsp;", " ").Replace("EUR", " ").Trim();

            return AlbumParsingHelper.ParsePrice(priceText);
        }

        private string ParsePhotoUrl(HtmlDocument htmlDocument)
        {
            var photoNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='photo_prod_container']/a");

            return photoNode?.GetAttributeValue("access_url", null);
        }

        private AlbumMediaType? ParseMediaType(HtmlDocument htmlDocument)
        {
            var mediaTypeText = GetNodeValue(htmlDocument, "//span[@class='cufonEb' and contains(text(), 'Media:')]")?.Split(':').Last().Trim();
            var mediaType = mediaTypeText?.Split(' ').FirstOrDefault();

            return AlbumParsingHelper.ParseMediaType(mediaType);
        }

        private string ParseLabel(HtmlDocument htmlDocument)
        {
            return GetNodeValue(htmlDocument, "//span[@class='cufonEb' and contains(text(), 'Label :')]//a");
        }

        private string ParsePress(HtmlDocument htmlDocument)
        {
            return GetNodeValue(htmlDocument, "//span[@class='cufonEb' and contains(text(), 'Press :')]")?.Split(':').Last().Trim();
        }

        private string ParseDescription(HtmlDocument htmlDocument)
        {
            var description = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='cufonEb' and contains(text(), 'Info :')]")?.InnerHtml;

            return description;
        }

        private AlbumStatus? ParseStatus(HtmlNode node)
        {
            var statusNode = node.SelectSingleNode(".//span[@class='inforestock']");

            if (statusNode != null)
            {
                var statusText = statusNode.InnerText.Trim();

                return AlbumParsingHelper.ParseAlbumStatus(statusText);
            }

            return null;
        }
    }