using MetalReleaseTracker.CoreDataService.Data.Entities.Enums;

namespace MetalReleaseTracker.CoreDataService.Dtos;

public record AlbumFilterDto
{
    public string? Name { get; init; }

    public decimal? MinPrice { get; init; }

    public decimal? MaxPrice { get; init; }

    public DateTime? ReleaseDateFrom { get; init; }

    public DateTime? ReleaseDateTo { get; init; }

    public Guid? BandId { get; init; }

    public Guid? DistributorId { get; init; }

    public AlbumStatus? Status { get; init; }
}