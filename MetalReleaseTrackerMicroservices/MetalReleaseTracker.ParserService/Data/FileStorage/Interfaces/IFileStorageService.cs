namespace MetalReleaseTracker.ParserService.Data.FileStorage.Interfaces;

public interface IFileStorageService
{
    Task UploadFileAsync(string filePath, Stream fileStream, CancellationToken cancellationToken);
}