using MetalReleaseTracker.CoreDataService.Dtos;
using MetalReleaseTracker.CoreDataService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MetalReleaseTracker.CoreDataService.Controllers;

[ApiController]
[Route("api/albums")]
public class AlbumController : ControllerBase
{
    private IAlbumService _albumService;

    public AlbumController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredAlbums([FromQuery] AlbumFilterDto filter)
    {
        var albums = await _albumService.IGetFilteredAlbums(filter);
        return Ok(albums);
    }
}