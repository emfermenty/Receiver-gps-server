using AutoMapper;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Entities;

namespace Infrastructure.Mapping.Profiles;

public class TrackProfile : Profile
{
    public TrackProfile()
    {
        CreateMap<Track, TrackEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());
        CreateMap<TrackEntity, Track>()
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}