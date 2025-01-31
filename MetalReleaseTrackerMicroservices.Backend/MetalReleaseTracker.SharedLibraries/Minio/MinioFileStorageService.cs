using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MetalReleaseTracker.SharedLibraries.Minio;

public class MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioFileStorageService(IMinioClient minioClient, IOptions<MinioFileStorageConfig> options)
    {
        _minioClient = minioClient;
        _bucketName = options.Value.BucketName;
    }

    public async Task UploadFileAsync(string filePach, Stream fileStream, CancellationToken cancellationToken)
    {
        await EnsureBucketExistsAsync(cancellationToken);

        await _minioClient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(filePach)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType("application/json"),
            cancellationToken);
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
    
    private async Task EnsureBucketExistsAsync(CancellationToken cancellationToken)
    {
        var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName), cancellationToken);

        if (!bucketExists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName), cancellationToken);
        }
    }
}