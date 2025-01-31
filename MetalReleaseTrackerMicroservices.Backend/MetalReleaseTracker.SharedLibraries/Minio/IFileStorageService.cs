namespace MetalReleaseTracker.SharedLibraries.Minio;

public interface IFileStorageService
{
    Task UploadFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken);
    
    Task<string> DownloadFileAsync(string filePath);
}