namespace MetalReleaseTracker.CatalogSyncService.Configurations;

public class MongoDbConfig
{
    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string ParsingSessionCollectionName { get; set; }

    public string RawAlbumsCollectionName { get; set; }

    public int RawAlbumsCollectionTTL { get; set; }

    public string ProcessedAlbumsCollectionName { get; set; }
}