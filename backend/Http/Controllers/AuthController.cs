using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using backend.Models;
using backend.Http.Requests;
using backend.Services;
using backend.Http.Responses;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Security.Claims;

namespace backend.Http.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public AuthController(TokenService tokenService, UserService userService, IMapper mapper)
        {
            _tokenService = tokenService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] AuthRequest request)
        {
            if (await _userService.UserExistsAsync(request.Username))
            {
                return BadRequest("User with this Username already exists");
            }

            var user = new User
            {
                Username = request.Username,
                Password = request.Password,
            };

            var createdUser = await _userService.CreateUserAsync(user);

            return Ok(new LoginResponse
            {
                Token = _tokenService.GenerateToken(createdUser),
                User = _mapper.Map<UserResponse>(createdUser)
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] AuthRequest request)
        {
            var user = await _userService.ValidateUserCredentialsAsync(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserResponse>(user)
            });
        }

        [HttpGet]
        [Authorize]
        [Route("me")]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            return Ok(new LoginResponse
            {
                Token = token,
                User = _mapper.Map<UserResponse>(user)
            });
        }
    }
}