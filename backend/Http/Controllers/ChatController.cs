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

        return Ok(chats.Select(c => new ChatUserResponse {
            Chat = new ChatResponse {
                Id = c.Id,
                Name = c.Name,
            },
            Users = c.ChatUsers.Select(cu => new UserResponse {
                Id = cu.User.Id,
                Username = cu.User.Username,
            }).ToList(),
        }));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Chat>> CreateChat([FromBody] ChatRequest request)
    {
        var user = await _userService.GetUserByIdAsync(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        var users = new List<User>() { user };

        if (request.Id != null)
        {
            var chat = await _chatService.GetChatByIdAsync(request.Id);
            if (chat != null)
            {
                return BadRequest("Chat already exists.");
            }
        }

        if (request.UserId != null)
        {
            var userToAdd = await _userService.GetUserByIdAsync(request.UserId.Value);
            if (userToAdd != null)
            {
                users.Add(userToAdd);
            }
        }

        var createdChat = await _chatService.CreateChatAsync(request.Id, users, true);

        return Ok(new ChatUserResponse {
            Chat = new ChatResponse {
                Id = createdChat.Id,
                Name = createdChat.Name,
            },
            Users = users.Select(u => new UserResponse {
                Id = u.Id,
                Username = u.Username,
            }).ToList(),
        });
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
                Chat = new ChatResponse {
                    Id = user.CurrentChatId,
                    Name = user.CurrentChat.Name,
                },
                User = new UserResponse {
                    Id = user.Id,
                    Username = user.Username,
                    CurrentChatId = user.CurrentChatId,
                },
                Users = new List<UserResponse> {
                    new UserResponse {
                        Id = user.Id,
                        Username = user.Username,
                        CurrentChatId = user.CurrentChatId,
                    },
                },
            });
        }

        var chatUser = await _chatService.AddUserToChatAsync(request.ChatId, userId);
        if (chatUser == null)
        {
            return BadRequest("Failed to join chat.");
        }

        user.CurrentChatId = request.ChatId;
        await _userService.UpdateUserAsync(user.Id, user);

        var chat = await _chatService.GetChatByIdAsync(request.ChatId);

        return Ok(new ChatUserResponse {
            Chat = new ChatResponse {
                Id = chatUser.ChatId,
                Name = chat.Name,
            },
            User = new UserResponse {
                Id = user.Id,
                Username = user.Username,
                CurrentChatId = user.CurrentChatId,
            },
            Users = chatUser.Chat.ChatUsers.Select(cu => new UserResponse {
                Id = cu.User.Id,
                Username = cu.User.Username,
                CurrentChatId = cu.User.CurrentChatId,
            }).ToList(),
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

        return Ok(new ChatResponse { Id = chat.Id, Name = chat.Name });
    }
}