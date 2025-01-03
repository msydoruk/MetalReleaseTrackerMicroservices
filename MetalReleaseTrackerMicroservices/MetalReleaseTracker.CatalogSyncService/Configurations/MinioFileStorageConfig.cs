namespace MetalReleaseTracker.CatalogSyncService.Configurations;

public class MinioFileStorageConfig
{
    public string Endpoint { get; set; }

    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string BucketName { get; set; }

    public string Region { get; set; }
}