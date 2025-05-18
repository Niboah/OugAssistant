using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;


namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{

    private readonly OugAssistant_DB.Features.Planning _context;

    public TaskController(OugAssistant_DB.Features.Planning context)
    {
        _context = context;
    }

    #region DTO

    public class OugTaskDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TaskPriority? Priority { get; set; }
        public Guid? GoalId { get; set; }
    }
    public class OugEventDto : OugTaskDto
    {
        public DateTime? EventDateTime { get; set; }
        public string? Place { get; set; }
    }

    public class OugMissionDto : OugTaskDto
    {
        public DateTime? DeadLine { get; set; }
    }

    public class OugRoutineDto : OugTaskDto
    {

        public HashSet<TimeOnly>[]? WeekTimes { get; set; }
    }
    #endregion

    #region GET

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OugTask>>> GetTasks()
    {
        return await _context.OugTasks.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OugTask>> GetTask(Guid id)
    {
        var task = await _context.OugTasks.FindAsync(id);
        if (task == null) return NotFound();
        return task;
    }

    #endregion

    #region POST

    [Route("Event")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateEvent([FromBody] OugEventDto dto)
    {
        if (dto.Name == null || dto.Priority == null || dto.GoalId == null || dto.EventDateTime == null)
            return BadRequest("Missing required fields for Event");

        var goal = await _context.OugGoal.FindAsync(dto.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");

        var task = new OugEvent(
            name: dto.Name,
            description: dto.Description,
            priority: dto.Priority.Value,
            goalId: goal.Id,
            goal: goal,
            eventDateTime: dto.EventDateTime.Value,
            place: dto.Place
        );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }
    
    [Route("Mission")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateMission([FromBody] OugMissionDto dto)
    {
        if (dto.Name == null || dto.Priority == null || dto.GoalId == null || dto.DeadLine == null)
            return BadRequest("Missing required fields for Mission");

        var goal = await _context.OugGoal.FindAsync(dto.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");

        var task = new OugMission(
            name: dto.Name,
            description: dto.Description,
            priority: dto.Priority.Value,
            goalId: goal.Id,
            goal: goal,
            deadLine: dto.DeadLine.Value
        );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }
    
    [Route("Routine")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateRoutine([FromBody] OugRoutineDto dto)
    {
        if (dto.Name == null || dto.Priority == null || dto.GoalId == null || dto.WeekTimes == null)
            return BadRequest("Missing required fields for Routine");

        var goal = await _context.OugGoal.FindAsync(dto.GoalId.Value);
        if (goal == null) return NotFound("Goal not found.");

        var task = new OugRoutine(
            name: dto.Name,
            description: dto.Description,
            priority: dto.Priority.Value,
            goalId: goal.Id,
            goal: goal,
            weekTimes: dto.WeekTimes
        );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    #endregion

    #region PATCH

    [Route("Event/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchEvent(Guid id, [FromBody] OugEventDto dto)
    {
        var task = await _context.OugTasks.OfType<OugEvent>().FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        if (dto.Name != null) task.Name = dto.Name;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.EventDateTime.HasValue) task.EventDateTime = dto.EventDateTime.Value;
        if (dto.Place != null) task.Place = dto.Place;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Route("Mission/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchMission(Guid id, [FromBody] OugMissionDto dto)
    {
        var task = await _context.OugTasks.OfType<OugMission>().FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        if (dto.Name != null) task.Name = dto.Name;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.DeadLine.HasValue) task.DeadLine = dto.DeadLine.Value;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [Route("Routine/{id}")]
    [HttpPatch]
    public async Task<IActionResult> PatchRoutine(Guid id, [FromBody] OugRoutineDto dto)
    {
        var task = await _context.OugTasks.OfType<OugRoutine>().FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        if (dto.Name != null) task.Name = dto.Name;
        if (dto.Description != null) task.Description = dto.Description;
        if (dto.Priority.HasValue) task.Priority = dto.Priority.Value;
        if (dto.WeekTimes != null) task.WeekTimes = dto.WeekTimes;

        await _context.SaveChangesAsync();
        return Ok();
    }

    #endregion

    #region DELETE
    [Route("Task/{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var task = await _context.OugTasks.FindAsync(id);
        if (task == null) return NotFound();

        _context.OugTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    #endregion

}
