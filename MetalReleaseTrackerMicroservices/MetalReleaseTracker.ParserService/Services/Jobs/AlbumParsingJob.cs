using Hangfire;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Interfaces;
using Newtonsoft.Json;

namespace MetalReleaseTracker.ParserService.Services.Jobs;

public class AlbumParsingJob
{
    private readonly Func<DistributorCode, IParser> _parserResolver;
    private readonly IAlbumParsedEventRepository _albumParsedEventRepository;
    private readonly ILogger<AlbumParsingJob> _logger;

    public AlbumParsingJob(
        Func<DistributorCode, IParser> parserResolver,
        IAlbumParsedEventRepository albumParsedEventRepository,
        ILogger<AlbumParsingJob> logger)
    {
        _parserResolver = parserResolver;
        _albumParsedEventRepository = albumParsedEventRepository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task RunParserJob(ParserDataSource parserDataSource, CancellationToken cancellationToken)
    {
        try
        {
            var parser = _parserResolver(parserDataSource.DistributorCode);
            await foreach (var albumParsedEvent in parser.ParseAsync(parserDataSource.ParsingUrl, cancellationToken))
            {
                await _albumParsedEventRepository.AddAsync(
                    albumParsedEvent.ParsingSessionId,
                    JsonConvert.SerializeObject(albumParsedEvent),
                    cancellationToken);

                _logger.LogInformation($"Added album parsed event to outbox storage. SKU: {albumParsedEvent.SKU}, Distributor: {albumParsedEvent.DistributorCode}.");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error occurred while parsing distributor: {parserDataSource.DistributorCode}");
            throw;
        }
    }
}