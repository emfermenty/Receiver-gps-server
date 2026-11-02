using AutoMapper;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Entities;

namespace Infrastructure.Mapping.Profiles;

public class GeoDataProfile : Profile
{
    public GeoDataProfile()
    {
        CreateMap<GeoData, GeoDataEntity>()
            .ForMember(dest => dest.EventType, opt => opt.Ignore())
            .ForMember(dest => dest.Sensor, opt => opt.Ignore());

        CreateMap<GeoDataEntity, GeoData>()
            .ForMember(dest => dest.EventType, opt => opt.Ignore())
            .ForMember(dest => dest.Sensor, opt => opt.Ignore());
    }
}
