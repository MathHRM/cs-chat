using AutoMapper;
using backend.Http.Responses;
using backend.Models;

namespace backend.src.Mappers;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Chat, ChatResponse>();
    }
}