using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using OugAssistant_APP.Interfaces.Planning;

namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class GoalController : ControllerBase
{
    private readonly IGoalServices _goalServices;
    private readonly ILogger<GoalController> _logger;

    public GoalController(IGoalServices goalServices, ILogger<GoalController> logger)
    {
        _goalServices = goalServices;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GoalAPIout>>> GetGoals()
    {
        try
        {
            _logger.LogDebug("GetGoals...");
            var result = await _goalServices.GetAllOugGoalAsync();
            _logger.LogDebug($"GetGoals completado. Total: {result?.Count() ?? 0}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            string message = $"Error en GetGoals: {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetGoal(Guid id)
    {
        try
        {
            _logger.LogDebug($"GetGoal con id: {id} ...");
            var goal = await _goalServices.GetOugGoalByIdAsync(id);

            if (goal == null)
            {
                _logger.LogWarning($"GetGoal {id} no encontrada.");
                return NotFound();
            }

            _logger.LogDebug($"GetGoal exitoso para id: {id}");
            return Ok(goal);
        }
        catch (Exception ex)
        {
            string message = $"Error en GetGoal({id}): {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }

    [HttpPost("")]
    public async Task<ActionResult<object>> CreateGoal([FromBody] GoalAPIin goal)
    {
        try
        {
            _logger.LogDebug("CreateGoal...");
            bool result = await _goalServices.AddOugGoalAsync(goal);
            _logger.LogDebug($"CreateGoal completado. Resultado: {result}");
            return result ? Ok(result) : Conflict(result);
        }
        catch (Exception ex)
        {
            string message = $"Error en CreateGoal: {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }

    [HttpPost("Finish/{id}")]
    public async Task<ActionResult<bool>> FinishGoal(Guid id)
    {
        try
        {
            _logger.LogDebug($"FinishGoal con id: {id}");
            bool result = await _goalServices.FinishGoal(id);
            _logger.LogDebug($"FinishGoal completado para id: {id}. Resultado: {result}");
            return result ? Ok(result) : Conflict(result);
        }
        catch (Exception ex)
        {
            string message = $"Error en FinishGoal({id}): {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<GoalAPIout>> UpdateGoal(Guid id, [FromBody] GoalAPIin goal)
    {
        if (id != goal.Id)
        {
            string warning = $"UpdateGoal: ID de la URL ({id}) no coincide con el cuerpo ({goal.Id})";
            _logger.LogWarning(warning);
            return BadRequest(warning);
        }

        try
        {
            _logger.LogDebug($"UpdateGoal para id: {id}");
            await _goalServices.UpdateOugGoalAsync(goal);
            _logger.LogDebug($"UpdateGoal completado para id: {id}");
            return Ok(goal);
        }
        catch (Exception ex)
        {
            string message = $"Error en UpdateGoal({id}): {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteGoal(Guid id)
    {
        try
        {
            _logger.LogDebug($"DeleteGoal con id: {id}");
            bool result = await _goalServices.DeleteOugGoalAsync(id);
            _logger.LogDebug($"DeleteGoal completado para id: {id}. Resultado: {result}");
            return Ok(result);
        }
        catch (Exception ex)
        {
            string message = $"Error en DeleteGoal({id}): {ex.Message}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, message);
        }
    }
}
