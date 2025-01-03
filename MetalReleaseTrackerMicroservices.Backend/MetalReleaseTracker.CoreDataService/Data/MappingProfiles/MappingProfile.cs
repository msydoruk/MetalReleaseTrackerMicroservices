using MetalReleaseTracker.CoreDataService.Data.Events;

namespace MetalReleaseTracker.CoreDataService.Data.MappingProfiles;

using AutoMapper;
using Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AlbumProcessedPublicationEvent, AlbumEntity>();
    }
}