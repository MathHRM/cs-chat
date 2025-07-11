using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using backend.Http.Requests;
using backend.Http.Responses;

namespace backend.Http.Controllers;

[ApiController]
[Route("api/chats")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;

    public ChatController(ChatService chatService, UserService userService)
    {
        _chatService = chatService;
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var chats = await _chatService.GetAllChatsAsync(int.Parse(userId));

        return Ok(chats);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Chat>> CreateChat([FromBody] ChatRequest request)
    {
        var createdChat = await _chatService.CreateChatAsync(request.Id);
        var users = new List<User>();

        if (createdChat == null)
        {
            return BadRequest("Chat already exists.");
        }

        if (request.UserId != null)
        {
            var user = await _userService.GetUserByIdAsync(request.UserId.Value);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            users.Add(user);
        }

        return Ok(new ChatResponse { Id = createdChat.Id, Users = users });
    }
}