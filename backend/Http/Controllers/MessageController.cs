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
[Route("api/messages")]
public class MessageController : ControllerBase
{
    private readonly MessageService _messageService;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public MessageController(MessageService messageService, UserService userService, IMapper mapper)
    {
        _messageService = messageService;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessages([FromQuery] int? lastMessageId = null)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetUserByIdAsync(int.Parse(userId));

        if (user == null)
        {
            return Unauthorized();
        }

        if (user.CurrentChatId == null)
        {
            return BadRequest("User does not have a current chat.");
        }

        var messages = await _messageService.GetMessagesAsync(user.CurrentChatId, lastMessageId);

        return Ok(messages.Select(m => _mapper.Map<MessageResponse>(m)).Reverse());
    }

    [HttpGet]
    [Route("guest")]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetGuestMessages([FromQuery] int? lastMessageId = null)
    {
        var messages = await _messageService.GetMessagesAsync("guest", lastMessageId);
        return Ok(messages.Select(m => _mapper.Map<MessageResponse>(m)).Reverse());
    }
}