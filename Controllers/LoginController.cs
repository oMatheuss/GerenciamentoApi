using GerenciamentoAPI.Helpers;
using GerenciamentoAPI.Models;
using GerenciamentoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GerenciamentoAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly GerenciamentoService _gerenciamentoService;
        private readonly JWTManager _jwtManager;

        public LoginController(ILogger<LoginController> logger, GerenciamentoService gerenciamentoService, JWTManager jwtManager)
        {
            _logger = logger;
            _gerenciamentoService = gerenciamentoService;
            _jwtManager = jwtManager;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req.Email.Length < 5 || !Regexes.Email.IsMatch(req.Email))
                return BadRequest("Email invalido!");

            User user;
            try
            {
                user = _gerenciamentoService.GetUser(req.Email);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (!PasswordHelper.CheckPassword(user.Password, req.Password))
                return BadRequest("Senha incorreta!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, req.Email),
                new Claim(ClaimTypes.Role, user.Roles),
            };

            var token = _jwtManager.GenerateToken(claims);

            return Ok(token);
        }

        [HttpPost("SignUp")]
        public IActionResult SignUp([FromBody] UserInsertRequest user)
        {
            try
            {
                _gerenciamentoService.SaveUser(user);
                _logger.Log(LogLevel.Information, $"Usuario {user.Name} inserido com sucesso!", user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
