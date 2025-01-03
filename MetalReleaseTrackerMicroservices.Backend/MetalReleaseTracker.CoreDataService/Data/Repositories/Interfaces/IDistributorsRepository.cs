public interface IDistributorsRepository
{
    Task<Guid> GetOrAddAsync(string distributorName);
}