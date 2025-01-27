﻿using Confluent.Kafka;
using MassTransit;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Dtos;
using MetalReleaseTracker.ParserService.Parsers.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.Parsers.Implementation;

public class DrakkarParser : IParser
{
    private readonly ILogger<DrakkarParser> _logger;

    public DistributorCode DistributorCode => DistributorCode.Drakkar;

    public DrakkarParser(ILogger<DrakkarParser> logger)
    {
        _logger = logger;
    }

    public async Task<PageParsedResult> ParseAsync(string parsingUrl, CancellationToken cancellationToken)
    {
        return new PageParsedResult();
    }
}