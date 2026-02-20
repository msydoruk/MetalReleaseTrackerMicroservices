using MetalReleaseTracker.ParserService.Domain.Models.Entities;
using MetalReleaseTracker.ParserService.Infrastructure.Data;
using MetalReleaseTracker.ParserService.Infrastructure.Data.Entities;
using MetalReleaseTracker.ParserService.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.DependencyInjection;
using TickerQ.Utilities;
using TickerQ.Utilities.Interfaces.Managers;

namespace MetalReleaseTracker.ParserService.Infrastructure.Scheduling.Extensions;

public static class TickerQExtensions
{
    public static IServiceCollection AddTickerQScheduler(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TickerQConnectionString")
            ?? throw new InvalidOperationException("TickerQConnectionString is not configured");

        var dashboardEnabled = configuration.GetValue<bool>("TickerQ:Dashboard:Enabled", true);
        var dashboardBasePath = configuration.GetValue<string>("TickerQ:Dashboard:BasePath");
        var dashboardUsername = configuration.GetValue<string>("TickerQ:Dashboard:Username");
        var dashboardPassword = configuration.GetValue<string>("TickerQ:Dashboard:Password");

        var parserDataSources = configuration.GetSection("ParserDataSources")
            .Get<List<ParserDataSource>>() ?? [];

        services.AddTickerQ<CustomTimeTicker, CustomCronTicker>(tickerOptions =>
        {
            tickerOptions.ConfigureScheduler(schedulerOptions =>
            {
                schedulerOptions.MaxConcurrency = Environment.ProcessorCount * 2;
                schedulerOptions.NodeIdentifier = Environment.MachineName;
                schedulerOptions.IdleWorkerTimeOut = TimeSpan.FromMinutes(2);
                schedulerOptions.FallbackIntervalChecker = TimeSpan.FromSeconds(30);
                schedulerOptions.SchedulerTimeZone = TimeZoneInfo.Utc;
            });

            tickerOptions.UseGZipCompression();
            tickerOptions.SetExceptionHandler<TickerQExceptionHandler>();

            tickerOptions.UseTickerSeeder(
                async (ICronTickerManager<CustomCronTicker> cronManager) =>
                {
                    foreach (var source in parserDataSources)
                    {
                        var request = TickerHelper.CreateTickerRequest(source);

                        await cronManager.AddAsync(new CustomCronTicker
                        {
                            Function = "CatalogueIndexJob",
                            Request = request,
                            Expression = "0 0 2 */3 * *",
                            Description = $"Catalogue index every 3 days for {source.Name}",
                            Retries = 2,
                            RetryIntervals = [30000, 60000],
                        });

                        await cronManager.AddAsync(new CustomCronTicker
                        {
                            Function = "AlbumDetailParsingJob",
                            Request = request,
                            Expression = "0 0 8 */3 * *",
                            Description = $"Album detail parsing every 3 days for {source.Name}",
                            Retries = 2,
                            RetryIntervals = [30000, 60000],
                        });
                    }
                });

            if (dashboardEnabled)
            {
                tickerOptions.AddDashboard(dashboard =>
                {
                    if (!string.IsNullOrEmpty(dashboardBasePath))
                    {
                        dashboard.SetBasePath(dashboardBasePath);
                    }

                    if (!string.IsNullOrEmpty(dashboardUsername) && !string.IsNullOrEmpty(dashboardPassword))
                    {
                        dashboard.WithBasicAuth(dashboardUsername, dashboardPassword);
                    }
                });
            }

            tickerOptions.AddOperationalStore(storeOptions =>
            {
                storeOptions.UseTickerQDbContext<ParserServiceTickerQDbContext>(dbContextOptions =>
                {
                    dbContextOptions.UseNpgsql(connectionString, cfg =>
                    {
                        cfg.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), ["40P01"]);
                    });
                });
            });
        });

        return services;
    }
}
