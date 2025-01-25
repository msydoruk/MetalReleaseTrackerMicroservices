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

    public async Task<AlbumEntity?> Get(Guid id)
    {
        return await _dbContext.Albums.Where(album => album.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddAsync(AlbumEntity entity)
    {
        await _dbContext.Albums.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(AlbumEntity entity)
    {
        var existingEntity = await Get(entity.Id);
        if (existingEntity != null)
        {
            _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            _dbContext.Albums.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var existingEntity = await Get(id);

        if (existingEntity != null)
        {
            _dbContext.Albums.Remove(existingEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}