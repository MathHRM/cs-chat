using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using backend.Models;
using backend.Http.Requests;
using backend.Services;
using backend.Http.Responses;

namespace backend.Http.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserService _userService;

        public AuthController(TokenService tokenService, UserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
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
                User = new UserResponse
                {
                    Id = createdUser.Id,
                    Username = createdUser.Username,
                }
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
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                }
            });
        }
    }
}