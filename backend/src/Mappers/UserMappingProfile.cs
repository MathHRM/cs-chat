using AutoMapper;
using backend.Http.Responses;
using backend.Models;

namespace backend.src.Mappers;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>();
    }
}