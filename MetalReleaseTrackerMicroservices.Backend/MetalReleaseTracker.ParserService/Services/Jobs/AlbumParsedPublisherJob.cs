using Hangfire;
using MassTransit;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Entities;
using MetalReleaseTracker.ParserService.Data.Entities.Enums;
using MetalReleaseTracker.ParserService.Data.FileStorage.Interfaces;
using MetalReleaseTracker.ParserService.Data.Repositories.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Models;
using Newtonsoft.Json;

namespace MetalReleaseTracker.ParserService.Services.Jobs;

public class AlbumParsedPublisherJob
{
    private readonly IAlbumParsedEventRepository _albumParsedEventRepository;
    private readonly IParsingSessionRepository _parsingSessionRepository;
    private readonly ITopicProducer<AlbumParsedPublicationEvent> _topicProducer;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<AlbumParsedPublisherJob> _logger;

    public AlbumParsedPublisherJob(
        IAlbumParsedEventRepository albumParsedEventRepository,
        IParsingSessionRepository parsingSessionRepository,
        IFileStorageService fileStorageService,
        ITopicProducer<AlbumParsedPublicationEvent> topicProducer,
        ILogger<AlbumParsedPublisherJob> logger)
    {
        _parsingSessionRepository = parsingSessionRepository;
        _albumParsedEventRepository = albumParsedEventRepository;
        _fileStorageService = fileStorageService;
        _topicProducer = topicProducer;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task RunPublisherJob(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var parsingSessions = await _parsingSessionRepository.GetParsedAsync(cancellationToken);

                if (!parsingSessions.Any())
                {
                    break;
                }

                foreach (var parsingSession in parsingSessions)
                {
                    var albumsToPublish = await _albumParsedEventRepository.GeAsync(parsingSession.Id, cancellationToken);

                    try
                    {
                        var storageFilePath = await UploadToFileStorageAsync(parsingSession.DistributorCode, albumsToPublish, cancellationToken);
                        await PublishEvent(parsingSession, storageFilePath, cancellationToken);
                        await _parsingSessionRepository.UpdateParsingStatus(parsingSession.Id, AlbumParsingStatus.Published, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Albums publishing failed. Parsing state: {parsingSession.Id}.");
                    }
                }
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error occurred while publishing albums to parser service topic.");
            throw;
        }
    }

    private async Task<string> UploadToFileStorageAsync(
        DistributorCode distributorCode,
        List<AlbumParsedEventEntity> albumsToPublish,
        CancellationToken cancellationToken)
    {
        var albumObjects = albumsToPublish.Select(album => JsonConvert.DeserializeObject<AlbumParsedEvent>(album.EventPayload));
        var serializedAlbums = JsonConvert.SerializeObject(albumObjects, Formatting.Indented);

        using var memoryStream = new MemoryStream();
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            await writer.WriteAsync(serializedAlbums);
            await writer.FlushAsync(cancellationToken);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        var filePath = $"{Guid.NewGuid()}/{distributorCode}.json";
        await _fileStorageService.UploadFileAsync(filePath, memoryStream, cancellationToken);

        return filePath;
    }

    private async Task PublishEvent(ParsingSessionEntity parsingSessionEntity, string storageFilePath, CancellationToken cancellationToken)
    {
        var albumParsedPublicationEvent = new AlbumParsedPublicationEvent
        {
            CreatedDate = DateTime.UtcNow,
            ParsingSessionId = parsingSessionEntity.Id,
            DistributorCode = parsingSessionEntity.DistributorCode,
            StorageFilePath = storageFilePath
        };

        await _topicProducer.Produce(albumParsedPublicationEvent, cancellationToken);

        _logger.LogInformation($"Published albums to parser service topic. Distributor: {albumParsedPublicationEvent.DistributorCode}.");
    }
}