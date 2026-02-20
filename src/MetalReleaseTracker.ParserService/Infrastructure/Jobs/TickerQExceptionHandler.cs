using TickerQ.Utilities.Enums;
using TickerQ.Utilities.Interfaces;

namespace MetalReleaseTracker.ParserService.Infrastructure.Jobs;

public class TickerQExceptionHandler : ITickerExceptionHandler
{
    private readonly ILogger<TickerQExceptionHandler> _logger;

    public TickerQExceptionHandler(ILogger<TickerQExceptionHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType)
    {
        _logger.LogError(
            exception,
            "TickerQ job {TickerId} of type {TickerType} failed.",
            tickerId,
            tickerType);

        return Task.CompletedTask;
    }

    public Task HandleCanceledExceptionAsync(Exception exception, Guid tickerId, TickerType tickerType)
    {
        _logger.LogWarning(
            "TickerQ job {TickerId} of type {TickerType} was canceled.",
            tickerId,
            tickerType);

        return Task.CompletedTask;
    }
}
