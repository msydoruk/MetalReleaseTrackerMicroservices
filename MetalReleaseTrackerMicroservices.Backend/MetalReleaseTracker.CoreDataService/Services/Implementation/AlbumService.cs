using MetalReleaseTracker.CoreDataService.Dtos;
using MetalReleaseTracker.CoreDataService.Services.Interfaces;

namespace MetalReleaseTracker.CoreDataService.Services.Implementation;

public class AlbumService : IAlbumService
{
    private readonly IAlbumRepository _albumRepository;

    public AlbumService(IAlbumRepository albumRepository)
    {
        _albumRepository = albumRepository;
    }

    public Task<List<AlbumDto>> IGetFilteredAlbums(AlbumFilterDto filter)
    {
        throw new NotImplementedException();
    }
}