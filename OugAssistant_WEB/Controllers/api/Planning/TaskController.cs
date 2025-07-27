using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using OugAssistant_APP.Interfaces.Planning;

namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskServices _taskServices;
    private readonly IGoalServices _goalServices;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskServices taskServices, IGoalServices goalServices, ILogger<TaskController> logger)
    {
        _taskServices = taskServices;
        _goalServices = goalServices;
        _logger = logger;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAPIout>>> GetTasks()
    {
        try
        {
            _logger.LogDebug($"GetTasks...");
            var tasks = await _taskServices.GetAllOugTaskAsync();
            _logger.LogDebug($"Total de tareas: {tasks.Count()}");
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetTasks");
            return StatusCode(500, "Error interno al obtener las tareas.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskAPIout>> GetTask(Guid id)
    {
        try
        {
            _logger.LogDebug($"GetTasks con id: {id} ...");
            var task = await _taskServices.GetOugTaskByIdAsync(id);
            if (task == null)
            {
                _logger.LogWarning($"Tarea con ID {id} no encontrada.");
                return NotFound();
            }
            return Ok(task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error en GetTask({id})");
            return StatusCode(500, "Error interno al obtener la tarea.");
        }
    }

    #endregion

    #region POST

    [HttpPost("Event")]
    public async Task<ActionResult<TaskAPIout>> CreateEvent([FromBody] EventAPIin task)
    {
        try
        {
            if (task.Name == null || task.GoalId == null || task.EventDateTime == null)
                return BadRequest("Faltan campos obligatorios para el evento.");

            var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
            if (goal == null)
                return NotFound("Goal no encontrado.");

            await _taskServices.AddOugTaskAsync(task);
            _logger.LogInformation($"Evento creado: {task.Id}");
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateEvent");
            return StatusCode(500, "Error al crear el evento.");
        }
    }

    [HttpPost("Mission")]
    public async Task<ActionResult<TaskAPIout>> CreateMission([FromBody] MissionAPIin task)
    {
        try
        {
            if (task.Name == null || task.GoalId == null || task.DeadLine == null)
                return BadRequest("Faltan campos obligatorios para la misión.");

            var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
            if (goal == null)
                return NotFound("Goal no encontrado.");

            await _taskServices.AddOugTaskAsync(task);
            _logger.LogInformation($"Misión creada: {task.Id}");
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateMission");
            return StatusCode(500, "Error al crear la misión.");
        }
    }

    [HttpPost("Routine")]
    public async Task<ActionResult<TaskAPIout>> CreateRoutine([FromBody] RoutineAPIin task)
    {
        try
        {
            if (task.Name == null || task.GoalId == null || task.WeekTimes == null)
                return BadRequest("Faltan campos obligatorios para la rutina.");

            var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
            if (goal == null)
                return NotFound("Goal no encontrado.");

            await _taskServices.AddOugTaskAsync(task);
            _logger.LogInformation($"Rutina creada: {task.Id}");
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CreateRoutine");
            return StatusCode(500, "Error al crear la rutina.");
        }
    }

    #endregion

    #region PATCH

    [HttpPatch("Event/{id}")]
    public async Task<IActionResult> PatchEvent(Guid id, [FromBody] EventAPIin task)
    {
        if (id != task.Id)
            return BadRequest("El ID no coincide.");

        try
        {
            await _taskServices.UpdateOugTaskAsync(task);
            _logger.LogInformation($"Evento actualizado: {id}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando evento {id}");
            return StatusCode(500, "Error al actualizar el evento.");
        }
    }

    [HttpPatch("Mission/{id}")]
    public async Task<IActionResult> PatchMission(Guid id, [FromBody] MissionAPIin task)
    {
        if (id != task.Id)
            return BadRequest("El ID no coincide.");

        try
        {
            await _taskServices.UpdateOugTaskAsync(task);
            _logger.LogInformation($"Misión actualizada: {id}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando misión {id}");
            return StatusCode(500, "Error al actualizar la misión.");
        }
    }

    [HttpPatch("Routine/{id}")]
    public async Task<IActionResult> PatchRoutine(Guid id, [FromBody] RoutineAPIin task)
    {
        if (id != task.Id)
            return BadRequest("El ID no coincide.");

        try
        {
            await _taskServices.UpdateOugTaskAsync(task);
            _logger.LogInformation($"Rutina actualizada: {id}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error actualizando rutina {id}");
            return StatusCode(500, "Error al actualizar la rutina.");
        }
    }

    [HttpPatch("Finish/{id}")]
    public async Task<IActionResult> FinishTask(Guid id)
    {
        try
        {
            bool success = await _taskServices.Finish(id);
            if (!success)
            {
                _logger.LogWarning($"Tarea no encontrada o ya finalizada: {id}");
                return NotFound("Tarea no encontrada o ya finalizada.");
            }

            _logger.LogInformation($"Tarea finalizada: {id}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al finalizar tarea {id}");
            return StatusCode(500, "Error al finalizar la tarea.");
        }
    }

    #endregion

    #region DELETE

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        try
        {
            bool success = await _taskServices.DeleteOugTaskAsync(id);
            if (!success)
            {
                _logger.LogWarning($"Tarea no encontrada o ya eliminada: {id}");
                return NotFound("Tarea no encontrada o ya eliminada.");
            }

            _logger.LogInformation($"Tarea eliminada: {id}");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error eliminando tarea {id}");
            return StatusCode(500, "Error al eliminar la tarea.");
        }
    }

    #endregion
}
