using TickerQ.Utilities.Base;
using TickerQ.Utilities.Enums;

namespace MetalReleaseTracker.CatalogSyncService.Services.Jobs;

public class TickerQJobFunctions
{
    private readonly AlbumProcessingJob _albumProcessingJob;
    private readonly AlbumProcessedPublisherJob _albumProcessedPublisherJob;

    public TickerQJobFunctions(
        AlbumProcessingJob albumProcessingJob,
        AlbumProcessedPublisherJob albumProcessedPublisherJob)
    {
        _albumProcessingJob = albumProcessingJob;
        _albumProcessedPublisherJob = albumProcessedPublisherJob;
    }

    [TickerFunction("AlbumProcessingJob", "0 0 */4 * * *", TickerTaskPriority.LongRunning)]
    public async Task RunAlbumProcessingJob(
        TickerFunctionContext context,
        CancellationToken cancellationToken)
    {
        context.CronOccurrenceOperations.SkipIfAlreadyRunning();
        await _albumProcessingJob.RunProcessingJob(cancellationToken);
    }

    [TickerFunction("AlbumProcessedPublisherJob", "0 0 */1 * * *")]
    public async Task RunAlbumProcessedPublisherJob(
        TickerFunctionContext context,
        CancellationToken cancellationToken)
    {
        context.CronOccurrenceOperations.SkipIfAlreadyRunning();
        await _albumProcessedPublisherJob.RunPublisherJob(cancellationToken);
    }
}
