using MetalReleaseTracker.CoreDataService.Data;
using MetalReleaseTracker.CoreDataService.Data.MappingProfiles;
using MetalReleaseTracker.ParserService.ServiceExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddKafka(builder.Configuration);

builder.Services.AddScoped<IAlbumRepository, AlbumRepository>();
builder.Services.AddScoped<IBandRepository, BandRepository>();
builder.Services.AddScoped<IDistributorsRepository, DistributorRepository>();

var parserServiceConnectionString = builder.Configuration.GetConnectionString("CoreDataServiceConnectionString");
builder.Services.AddDbContext<CoreDataServiceDbContext>(options =>
{
    options.UseNpgsql(parserServiceConnectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDataServiceDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
