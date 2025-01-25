using MetalReleaseTracker.CoreDataService.Data.Entities;

public interface IAlbumRepository
{
    Task<AlbumEntity?> Get(Guid id);

    Task AddAsync(AlbumEntity entity);

    Task UpdateAsync(AlbumEntity entity);

    Task DeleteAsync(Guid id);
}