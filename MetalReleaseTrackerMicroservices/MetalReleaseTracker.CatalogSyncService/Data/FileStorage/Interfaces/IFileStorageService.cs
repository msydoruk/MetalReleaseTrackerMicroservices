namespace MetalReleaseTracker.CatalogSyncService.Data.FileStorage.Interfaces;

public interface IFileStorageService
{
    Task<string> DownloadFileAsync(string filePath);
}