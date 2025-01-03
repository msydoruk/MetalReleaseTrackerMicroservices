using MetalReleaseTracker.CoreDataService.Data.Entities;

public interface IAlbumRepository
{
    Task AddAsync(AlbumEntity entity);

    Task UpdateAsync(AlbumEntity entity);

    Task DeleteAsync(Guid id);
}