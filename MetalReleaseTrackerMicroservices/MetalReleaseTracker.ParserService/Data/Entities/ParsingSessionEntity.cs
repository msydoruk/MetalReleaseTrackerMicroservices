using System.ComponentModel.DataAnnotations;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Data.Entities.Enums;

namespace MetalReleaseTracker.ParserService.Data.Entities;

public class ParsingSessionEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DistributorCode DistributorCode { get; set; }

    [Required]
    public string PageToProcess { get; set; }

    [Required]
    public DateTime LastUpdatedDate { get; set; }

    public AlbumParsingStatus ParsingStatus { get; set; }
}