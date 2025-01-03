using MetalReleaseTracker.CoreDataService.Data;
using MetalReleaseTracker.CoreDataService.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class AlbumRepository : IAlbumRepository
{
    private readonly CoreDataServiceDbContext _dbContext;

    public AlbumRepository(CoreDataServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AlbumEntity entity)
    {
        await _dbContext.Albums.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(AlbumEntity entity)
    {
        _dbContext.Albums.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        _dbContext.Albums.Remove(new AlbumEntity { Id = id });
        await _dbContext.SaveChangesAsync();
    }
}