using AutoMapper;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Entities;

namespace Infrastructure.Mapping.Profiles;

public class SensorProfile : Profile
{
    public SensorProfile()
    {
        CreateMap<Sensor, SensorEntity>()
            .ForMember(dest => dest.Tracks, opt => opt.Ignore())
            .ForMember(dest => dest.GeoData, opt => opt.Ignore());
        
        CreateMap<SensorEntity, Sensor>()
            .ForMember(dest => dest.Tracks, opt => opt.Ignore())
            .ForMember(dest => dest.GeoData, opt => opt.Ignore());
    }
}