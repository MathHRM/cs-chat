using AutoMapper;
using backend.Http.Responses;
using backend.Models;

namespace backend.src.Mappers;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageResponse>();
    }
}