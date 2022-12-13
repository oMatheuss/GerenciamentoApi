using GerenciamentoAPI.Helpers;
using GerenciamentoAPI.Models;
using GerenciamentoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerenciamentoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly ILogger<ActivityController> _logger;
        private readonly GerenciamentoService _gerenciamentoService;

        public ActivityController(ILogger<ActivityController> logger, GerenciamentoService gerenciamentoService)
        {
            _logger = logger;
            _gerenciamentoService = gerenciamentoService;
        }

        [HttpGet]
        public IEnumerable<BaseActivity> Activities()
        {
            return _gerenciamentoService.ListActivities();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                _gerenciamentoService.DeleteActivity(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Status")]
        public IActionResult Put([FromBody] ActivityStatusRequest req)
        {
            try
            {
                _gerenciamentoService.SetActivityStatus(req.ActivityId, req.Status);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("Users")]
        public IEnumerable<BaseUser> Users(int activityId)
        {
            return _gerenciamentoService.ListActivityUsers(activityId);
        }

        [HttpPost]
        public IActionResult Activity(ActivityInsertRequest req)
        {
            try
            {
                var id = JWTManager.GetId(User);
                _gerenciamentoService.SaveActivity(req, Convert.ToInt32(id));
                _logger.Log(LogLevel.Information, $"Atividade {req.Name} inserido com sucesso!", req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("User")]
        public IActionResult Post(ActivityUserRequest req)
        {
            try
            {
                _gerenciamentoService.AddUserToActivity(req.ActivityId, req.Email);
                _logger.Log(LogLevel.Information, $"Usuario {req.Email} inserido com sucesso na atividade {req.ActivityId}!", req);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
