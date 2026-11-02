using AutoMapper;
using mapsence.Domain.Models;
using mapsence.Infrastructure.Entities;

namespace Infrastructure.Mapping.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>();
    }
}
