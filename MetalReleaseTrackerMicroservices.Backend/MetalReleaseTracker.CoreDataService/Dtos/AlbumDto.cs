using MetalReleaseTracker.CoreDataService.Data.Entities.Enums;

namespace MetalReleaseTracker.CoreDataService.Dtos;

public class AlbumDto
{
    public Guid Id { get; set; }

    public Guid DistributorName { get; set; }

    public Guid BandName { get; set; }

    public string SKU { get; set; }

    public string Name { get; set; }

    public DateTime ReleaseDate { get; set; }

    public string? Genre { get; set; }

    public float Price { get; set; }

    public string PurchaseUrl { get; set; }

    public string PhotoUrl { get; set; }

    public AlbumMediaType? Media { get; set; }

    public string Label { get; set; }

    public string Press { get; set; }

    public string? Description { get; set; }

    public AlbumStatus? Status { get; set; }
}