using MetalReleaseTracker.CoreDataService.Data;
using MetalReleaseTracker.CoreDataService.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class BandRepository : IBandRepository
{
    private readonly CoreDataServiceDbContext _dbContext;

    public BandRepository(CoreDataServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> GetOrAddAsync(string bandName)
    {
        var existingBandEntity = await _dbContext.Bands.FirstOrDefaultAsync(b => b.Name == bandName);

        if (existingBandEntity != null)
        {
            return existingBandEntity.Id;
        }

        var newBandEntity = new BandEntity { Name = bandName };
        await _dbContext.Bands.AddAsync(newBandEntity);
        await _dbContext.SaveChangesAsync();

        return newBandEntity.Id;
    }
}