using MetalReleaseTracker.CoreDataService.Data;
using MetalReleaseTracker.CoreDataService.Data.Entities;
using Microsoft.EntityFrameworkCore;

public class DistributorRepository : IDistributorsRepository
{
    private readonly CoreDataServiceDbContext _dbContext;

    public DistributorRepository(CoreDataServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> GetOrAddAsync(string distributorName)
    {
        var existingBandEntity = await _dbContext.Distributors.FirstOrDefaultAsync(b => b.Name == distributorName);

        if (existingBandEntity != null)
        {
            return existingBandEntity.Id;
        }

        var newDistributorEntity = new DistributorEntity { Name = distributorName };
        await _dbContext.Distributors.AddAsync(newDistributorEntity);
        await _dbContext.SaveChangesAsync();

        return newDistributorEntity.Id;
    }
}
