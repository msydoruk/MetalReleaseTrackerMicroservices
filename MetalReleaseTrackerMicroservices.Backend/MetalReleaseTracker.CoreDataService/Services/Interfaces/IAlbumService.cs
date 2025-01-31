using MetalReleaseTracker.CoreDataService.Dtos;

namespace MetalReleaseTracker.CoreDataService.Services.Interfaces;

public interface IAlbumService
{
    Task<List<AlbumDto>> IGetFilteredAlbums(AlbumFilterDto filter);
}