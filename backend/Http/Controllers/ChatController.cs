using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend.Http.Requests;
using backend.Http.Responses;
using AutoMapper;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/chats")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public ChatController(ChatService chatService, UserService userService, IMapper mapper)
    {
        _chatService = chatService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var chats = await _chatService.GetAllChatsAsync(int.Parse(userId));

        return Ok(chats.Select(c => new ChatUserResponse {
            Chat = _mapper.Map<ChatResponse>(c),
            Users = c.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
        }));
    }

    [HttpPost]
    [Authorize]
    [Route("join")]
    public async Task<ActionResult<ChatUserResponse>> JoinChat([FromBody] JoinChatRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var user = await _userService.GetUserByIdAsync(userId);

        if (user.CurrentChatId == request.ChatId)
        {
            return Ok(new ChatUserResponse {
                Chat = _mapper.Map<ChatResponse>(user.CurrentChat),
                User = _mapper.Map<UserResponse>(user),
                Users = user.CurrentChat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
            });
        }

        var chat = await _chatService.JoinChatAsync(request.ChatId, userId);
        if (chat == null)
        {
            return BadRequest("Failed to join chat.");
        }

        user.CurrentChatId = chat.Id;
        await _userService.UpdateUserAsync(user.Id, user);

        return Ok(new ChatUserResponse {
            Chat = _mapper.Map<ChatResponse>(chat),
            User = _mapper.Map<UserResponse>(user),
            Users = chat.ChatUsers.Select(cu => _mapper.Map<UserResponse>(cu.User)).ToList(),
        });
    }

    [HttpGet]
    [Authorize]
    [Route("{chatId}")]
    public async Task<ActionResult<Chat>> GetChat(string chatId)
    {
        var chat = await _chatService.GetChatByIdAsync(chatId);

        if (chat == null)
        {
            return NotFound("Chat not found.");
        }

        return Ok(_mapper.Map<ChatResponse>(chat));
    }
}