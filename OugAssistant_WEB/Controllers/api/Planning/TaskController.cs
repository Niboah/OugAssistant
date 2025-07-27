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

    public TaskController(ITaskServices taskServices, IGoalServices goalServices)
    {
        _taskServices = taskServices;
        _goalServices = goalServices;
    }

    #region GET

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskAPIout>>> GetTasks()
    {
        var tasks = await _taskServices.GetAllOugTaskAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskAPIout>> GetTask(Guid id)
    {
        var task = await _taskServices.GetOugTaskByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    #endregion

    #region POST

    [Route("Event")]
    [HttpPost]
    public async Task<ActionResult<TaskAPIout>> CreateEvent([FromBody] EventAPIin task)
    {
        if (task.Name == null || task.GoalId == null || task.EventDateTime == null)
            return BadRequest("Missing required fields for Event");

        var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");
        await _taskServices.AddOugTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [Route("Mission")]
    [HttpPost]
    public async Task<ActionResult<TaskAPIout>> CreateMission([FromBody] MissionAPIin task)
    {
        if (task.Name == null || task.GoalId == null || task.DeadLine == null)
            return BadRequest("Missing required fields for Mission");

        var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");

        await _taskServices.AddOugTaskAsync(task);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [Route("Routine")]
    [HttpPost]
    public async Task<ActionResult<TaskAPIout>> CreateRoutine([FromBody] RoutineAPIin task)
    {
        if (task.Name == null || task.GoalId == null || task.WeekTimes == null)
            return BadRequest("Missing required fields for Routine");

        var goal = await _goalServices.GetOugGoalByIdAsync(task.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");

        await _taskServices.AddOugTaskAsync(task);

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    #endregion

    #region PATCH

    [Route("Event/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchEvent(Guid id, [FromBody] EventAPIin task)
    {
        if (id != task.Id) return new BadRequestResult();
        await _taskServices.UpdateOugTaskAsync(task);
        return Ok();
    }

    [Route("Mission/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchMission(Guid id, [FromBody] MissionAPIin task)
    {
        if (id != task.Id) return new BadRequestResult();
        await _taskServices.UpdateOugTaskAsync(task);
        return Ok();
    }

    [Route("Routine/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchRoutine(Guid id, [FromBody] RoutineAPIin task)
    {
        if (id != task.Id) return new BadRequestResult();
        await _taskServices.UpdateOugTaskAsync(task);
        return Ok();
    }

    [Route("Finish/{id}")]
    [HttpPatch]
    public async Task<IActionResult> FinishTask(Guid id)
    {
        var task = await _taskServices.Finish(id);
        if (!task) return NotFound("Task not found or already finished.");
        return Ok();
    }

    #endregion

    #region DELETE
    [Route("{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task  = await _taskServices.DeleteOugTaskAsync(id);
        if (!task) return NotFound("Task not found or already finished.");
        return Ok();
    }
    #endregion
}