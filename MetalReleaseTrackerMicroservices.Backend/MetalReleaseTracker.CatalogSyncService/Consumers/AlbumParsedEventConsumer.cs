using MassTransit;
using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MetalReleaseTracker.CatalogSyncService.Data.Events;
using MetalReleaseTracker.CatalogSyncService.Data.FileStorage.Interfaces;
using MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace MetalReleaseTracker.CatalogSyncService.Consumers;

public class AlbumParsedEventConsumer : IConsumer<AlbumParsedPublicationEvent>
{
    private readonly IParsingSessionRepository _parsingSessionRepository;
    private readonly IRawAlbumRepository _rawAlbumRepository;
    private readonly IFileStorageService _fileStorageService;
    private ILogger<AlbumParsedEventConsumer> _logger;

    public AlbumParsedEventConsumer(
        IParsingSessionRepository parsingSessionRepository,
        IRawAlbumRepository rawAlbumRepository,
        IFileStorageService fileStorageService,
        ILogger<AlbumParsedEventConsumer> logger)
    {
        _parsingSessionRepository = parsingSessionRepository;
        _rawAlbumRepository = rawAlbumRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AlbumParsedPublicationEvent> context)
    {
        try
        {
            var albumParsedPublicationEvent = context.Message;

            var parsingSessionEntity = new ParsingSessionEntity
            {
                Id = albumParsedPublicationEvent.ParsingSessionId,
                DistributorCode = albumParsedPublicationEvent.DistributorCode,
                CreatedDate = albumParsedPublicationEvent.CreatedDate
            };
            var rawAlbumJson = await _fileStorageService.DownloadFileAsync(albumParsedPublicationEvent.StorageFilePath);
            var rawAlbumEntities = JsonConvert.DeserializeObject<List<RawAlbumEntity>>(rawAlbumJson);

            await _parsingSessionRepository.AddAsync(parsingSessionEntity);
            await _rawAlbumRepository.AddAsync(rawAlbumEntities);
            await _parsingSessionRepository.UpdateProcessingStatusAsync(albumParsedPublicationEvent.ParsingSessionId, ParsingSessionProcessingStatus.Commited);

            _logger.LogInformation($"Created new parsing session and added albums to raw collection. Distributor: {albumParsedPublicationEvent.DistributorCode}.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error occurred while consuming parsing session.");
            throw;
        }
    }
}