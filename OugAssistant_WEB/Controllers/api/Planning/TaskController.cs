using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;
using static OugAssistant_WEB.Controllers.api.Planning.GoalController;
using static OugAssistant_WEB.Controllers.api.Planning.TaskController;


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

    public class OugTaskDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required TaskPriority Priority { get; set; }
        public required Guid GoalId { get; set; }
    }

    public class OugEventDto : OugTaskDto {
        public required DateTime EventDateTime { get; set; }
        public string? Place { get; set; }
    }

    public class OugMissionDto : OugTaskDto
    {
        public required DateTime DeadLine { get; set; }
    }

    public class OugRoutineDto : OugTaskDto
    {
        public required HashSet<TimeOnly>[] WeekTimes { get; set; }
    }

    // GET: api/Task
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OugTask>>> GetTasks()
    {
        return await _context.OugTasks
            .ToListAsync();
    }

    // GET: api/Task/id
    [HttpGet("{id}")]
    public async Task<ActionResult<OugTask>> GetTask(Guid id)
    {
        var task = await _context.OugTasks.FindAsync(id);

        if (task == null)
        {
            return NotFound();
        }

        return task;
    }

    // POST: api/Task/Event
    [Route("Event")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateEvent([FromBody] OugEventDto dto)
    {
        OugGoal goal = await _context.OugGoal.FindAsync(dto.GoalId);
        if (goal == null) {
            throw new NullReferenceException();
        }
        OugEvent task = new OugEvent(
                    name: dto.Name,
                    description: dto.Description,
                    priority: dto.Priority,
                    goalId: goal.Id,
                    goal: goal,
                    eventDateTime: dto.EventDateTime,
                    place: dto.Place
                );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTask", new { id = task.Id }, task);
    }

    // POST: api/Task/Mission
    [Route("Mission")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateMission([FromBody] OugMissionDto dto)
    {
        OugGoal goal = await _context.OugGoal.FindAsync(dto.GoalId);
        if (goal == null)
        {
            throw new NullReferenceException();
        }
        OugMission task = new OugMission(
                    name: dto.Name,
                    description: dto.Description,
                    priority: dto.Priority,
                    goalId: goal.Id,
                    goal: goal,
                    deadLine: dto.DeadLine
                );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTask", new { id = task.Id }, task);
    }

    // POST: api/Task/Routine
    [Route("Routine")]
    [HttpPost]
    public async Task<ActionResult<OugTask>> CreateRoutine([FromBody] OugRoutineDto dto)
    {
        OugGoal goal = await _context.OugGoal.FindAsync(dto.GoalId);
        if (goal == null)
        {
            throw new NullReferenceException();
        }

        OugRoutine task = new OugRoutine(
                    name: dto.Name,
                    description: dto.Description,
                    priority: (TaskPriority)dto.Priority,
                    goalId: goal.Id,
                    goal: goal,
                    weekTimes: dto.WeekTimes
                );

        _context.OugTasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetTask", new { id = task.Id }, task);
    }

    // DELETE: api/Task/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal(Guid id)
    {
        var task = await _context.OugTasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.OugTasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
