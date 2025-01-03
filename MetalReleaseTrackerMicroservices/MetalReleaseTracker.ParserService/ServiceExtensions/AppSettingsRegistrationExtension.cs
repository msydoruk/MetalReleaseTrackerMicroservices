using MetalReleaseTracker.ParserService.Configurations;

namespace MetalReleaseTracker.ParserService.ServiceExtensions;

public static class AppSettingsRegistrationExtension
{
    public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GeneralParserSettings>(configuration.GetSection("GeneralParserSettings"));
        services.Configure<AlbumParsedPublisherJobSettings>(configuration.GetSection("AlbumParsedPublisherJob"));
        services.Configure<List<ParserDataSource>>(configuration.GetSection("ParserDataSources"));
        services.Configure<MinioFileStorageConfig>(configuration.GetSection("Minio"));

        return services;
    }
}