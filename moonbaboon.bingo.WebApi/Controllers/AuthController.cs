using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using moonbaboon.bingo.Core.IServices;
using moonbaboon.bingo.WebApi.DTOs;

namespace moonbaboon.bingo.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _service;

        public AuthController(IUserService service, IConfiguration configuration, IAuthService authService)
        {
            _service = service;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost(nameof(LoginSwagger))]
        public ActionResult<AuthResponse> LoginSwagger(UserDtos.LoginDto loginInformation)
        {
            var salt = _service.GetSalt(_service.GetUserIdByUsername(loginInformation.Username));
            var password = BCrypt.Net.BCrypt.HashPassword(loginInformation.Password, salt);
            var user = _service.Login(loginInformation.Username, password);
            if (user == null)
                return BadRequest("User does not exist");

            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            return Ok(new AuthResponse {UUID = user.Id!, JWT = _authService.EncodeJwt(user, tokenKey)});
        }

        [HttpPost]
        public ActionResult Login(UserDtos.LoginDto loginInformation)
        {
            var user = _service.Login(loginInformation.Username, loginInformation.Password);
            if (user == null)
                return BadRequest("User does not exist");

            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            return Ok(new AuthResponse {UUID = user.Id!, JWT = _authService.EncodeJwt(user, tokenKey)});
        }

        public class AuthResponse
        {
            public string UUID { get; set; }
            public string JWT { get; set; }
        }
    }
}