using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace MetalReleaseTracker.ParserService.Data.Entities;

public class AlbumParsedEventEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ParsingSessionId { get; set; }

    [ForeignKey("ParsingSessionId")]
    public ParsingSessionEntity ParsingSession { get; set; }

    [Required]
    public string EventPayload { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; }
}