using MetalReleaseTracker.ParserService.Data.Entities;
using MetalReleaseTracker.ParserService.Parsers.Models;
using Microsoft.EntityFrameworkCore;

namespace MetalReleaseTracker.ParserService.Data;

public class ParserServiceDbContext : DbContext
{
    public DbSet<AlbumParsedEventEntity> AlbumParsedEvents { get; set; }

    public DbSet<ParsingSessionEntity> ParsingSessions { get; set; }

    public ParserServiceDbContext(DbContextOptions<ParserServiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AlbumParsedEventEntity>();
        modelBuilder.Entity<ParsingSessionEntity>();
    }
}