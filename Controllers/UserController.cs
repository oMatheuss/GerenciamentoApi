using GerenciamentoAPI.Helpers;
using GerenciamentoAPI.Models;
using GerenciamentoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly GerenciamentoService _gerenciamentoService;

        public UserController(ILogger<UserController> logger, GerenciamentoService gerenciamentoService)
        {
            _logger = logger;
            _gerenciamentoService = gerenciamentoService;
        }

        [HttpGet]
        public BaseUser Get()
        {
            var id = JWTManager.GetId(User);
            var user = _gerenciamentoService.GetUser(Convert.ToInt32(id));
            return new BaseUser()
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
            };
        }

        [HttpGet("Activities")]
        public IEnumerable<BaseActivity> Activities()
        {
            var id = JWTManager.GetId(User);
            return _gerenciamentoService.ListUserActivities(Convert.ToInt32(id));
        }

        [HttpGet("ActivitiesStatus")]
        public ActivitiesStatus ActivitiesStatus()
        {
            var id = JWTManager.GetId(User);
            return _gerenciamentoService.GetActivitiesStatus(Convert.ToInt32(id));
        }
    }
}