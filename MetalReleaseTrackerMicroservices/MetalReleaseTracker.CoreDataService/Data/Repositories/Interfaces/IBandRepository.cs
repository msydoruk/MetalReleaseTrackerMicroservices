public interface IBandRepository
{
    Task<Guid> GetOrAddAsync(string bandName);
}