﻿using AutoMapper;
using FluentValidation;
using Hangfire;
using MetalReleaseTracker.CatalogSyncService.Configurations;
using MetalReleaseTracker.CatalogSyncService.Data.Entities;
using MetalReleaseTracker.CatalogSyncService.Data.Entities.Enums;
using MetalReleaseTracker.CatalogSyncService.Data.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace MetalReleaseTracker.CatalogSyncService.Services.Jobs;

public class AlbumProcessingJob
{
    private readonly IParsingSessionRepository _parsingSessionRepository;
    private readonly IRawAlbumRepository _rawAlbumRepository;
    private readonly IValidator<RawAlbumEntity> _rawAlbumValidator;
    private readonly IAlbumProcessedRepository _albumProcessedRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AlbumProcessingJob> _logger;

    public AlbumProcessingJob(
        IParsingSessionRepository parsingSessionRepository,
        IRawAlbumRepository rawAlbumRepository,
        IValidator<RawAlbumEntity> rawAlbumValidator,
        IAlbumProcessedRepository albumProcessedRepository,
        IMapper mapper,
        ILogger<AlbumProcessingJob> logger)
    {
        _parsingSessionRepository = parsingSessionRepository;
        _rawAlbumRepository = rawAlbumRepository;
        _rawAlbumValidator = rawAlbumValidator;
        _albumProcessedRepository = albumProcessedRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task RunProcessingJob(CancellationToken cancellationToken)
    {
        try
        {
            var parsingSessions = await _parsingSessionRepository.GetUnProcessedAsync();

            if (!parsingSessions.Any())
            {
                _logger.LogInformation("No parsing sessions found.");
            }

            foreach (var parsingSession in parsingSessions)
            {
                await ProcessParsingSessionAsync(parsingSession, cancellationToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error occurred while processing albums.");
            throw;
        }
    }

    private async Task ProcessParsingSessionAsync(ParsingSessionEntity parsingSession,
        CancellationToken cancellationToken)
    {
        try
        {
            var rawAlbumsForParsingSession =
                await _rawAlbumRepository.GetBatchByParsingSessionIdAsync(parsingSession.Id);

            foreach (var rawAlbum in rawAlbumsForParsingSession)
            {
                await ProcessRawAlbumAsync(rawAlbum, cancellationToken);
            }

            var skusForParsingSession = new HashSet<string>(rawAlbumsForParsingSession.Select(album => album.SKU));
            await MarkDeletedAlbumsByDistributorAsync(parsingSession.DistributorCode, skusForParsingSession, cancellationToken);
            await _parsingSessionRepository.UpdateProcessingStatusAsync(parsingSession.Id, ParsingSessionProcessingStatus.Processed);

            _logger.LogInformation($"Processed {parsingSession.Id}.");
        }
        catch (Exception exception)
        {
            await _parsingSessionRepository.UpdateProcessingStatusAsync(parsingSession.Id, ParsingSessionProcessingStatus.Failed);

            _logger.LogError(exception, $"Error processing Parsing Session: {parsingSession.Id}");
            throw;
        }
    }

    private async Task ProcessRawAlbumAsync(RawAlbumEntity rawAlbumEntity, CancellationToken cancellationToken)
    {
        try
        {
            var validationResult = await _rawAlbumValidator.ValidateAsync(rawAlbumEntity, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            AlbumProcessedEntity? processedAlbum =
                await _albumProcessedRepository.GetBySkuAsync(rawAlbumEntity.SKU, cancellationToken);
            if (processedAlbum != null)
            {
                await UpdateProcessedAlbumAsync(processedAlbum, rawAlbumEntity, cancellationToken);
            }
            else
            {
                await AddProcessedAlbumAsync(rawAlbumEntity, cancellationToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                $"Album processing failed. SKU: {rawAlbumEntity.SKU}, Distributor: {rawAlbumEntity.DistributorCode}.");
        }
    }

    private async Task MarkDeletedAlbumsByDistributorAsync(DistributorCode distributorCode, HashSet<string> skusForParsingSession, CancellationToken cancellationToken)
    {
        var processedSkus = await _albumProcessedRepository.GetSkusByDistributorAsync(distributorCode, cancellationToken);

        var deletedSkus = processedSkus.Except(skusForParsingSession).ToList();

        foreach (var sku in deletedSkus)
        {
            await _albumProcessedRepository.UpdateStatusBySkuAsync(sku, AlbumProcessedStatus.Deleted, cancellationToken);
            _logger.LogInformation($"Deleted sku: {sku}");
        }
    }

    private async Task UpdateProcessedAlbumAsync(AlbumProcessedEntity processedAlbum, RawAlbumEntity rawAlbum, CancellationToken cancellationToken)
    {
        var changedFields = processedAlbum.GetChangedFields(rawAlbum);

        var isUpdated = changedFields.Any();
        if (isUpdated)
        {
            changedFields.Add(nameof(processedAlbum.ProcessedStatus), AlbumProcessedStatus.Updated);
            changedFields.Add(nameof(processedAlbum.LastUpdateDate), DateTime.UtcNow);
        }
        else
        {
            changedFields.Add(nameof(processedAlbum.LastCheckedDate), DateTime.UtcNow);
        }

        await _albumProcessedRepository.UpdateAsync(processedAlbum.Id, changedFields, cancellationToken);

        var action = isUpdated ? "Updated" : "Verified";
        _logger.LogInformation(
            $"{action} album in processed collection. SKU: {processedAlbum.SKU}, Distributor: {processedAlbum.DistributorCode}.");
    }

    private async Task AddProcessedAlbumAsync(RawAlbumEntity rawAlbum, CancellationToken cancellationToken)
    {
        var processedAlbum = _mapper.Map<RawAlbumEntity, AlbumProcessedEntity>(rawAlbum);
        processedAlbum.ProcessedStatus = AlbumProcessedStatus.New;

        await _albumProcessedRepository.AddAsync(processedAlbum, cancellationToken);

        _logger.LogInformation($"Added album to processed collection. SKU: {processedAlbum.SKU}, Distributor: {processedAlbum.DistributorCode}.");
    }
}