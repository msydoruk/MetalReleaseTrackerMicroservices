using MetalReleaseTracker.CatalogSyncService.Configurations;
using MetalReleaseTracker.CatalogSyncService.Data.FileStorage.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MetalReleaseTracker.CatalogSyncService.Data.FileStorage;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioFileStorageConfig> options)
    {
        _minioClient = minioClient;
        _bucketName = options.Value.BucketName;
    }

    public async Task<string> DownloadFileAsync(string filePath)
    {
        using var memoryStream = new MemoryStream();

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(filePath)
            .WithCallbackStream(stream => stream.CopyToAsync(memoryStream)));

        memoryStream.Position = 0;
        using var streamReader = new StreamReader(memoryStream);
        var content = await streamReader.ReadToEndAsync();

        return content;
    }
}