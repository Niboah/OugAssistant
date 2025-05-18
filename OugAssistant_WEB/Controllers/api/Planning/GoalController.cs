using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class GoalController : ControllerBase
{

    private readonly OugAssistant_DB.Features.Planning _context;

    public GoalController(OugAssistant_DB.Features.Planning context)
    {
        _context = context;
    }

    public class GoalDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    // GET: api/Goal
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OugGoal>>> GetGoals()
    {
        return await _context.OugGoal
            .ToListAsync();
    }

    // GET: api/Goal/id
    [HttpGet("{id}")]
    public async Task<ActionResult<OugGoal>> GetGoal(Guid id)
    {
        var goal = await _context.OugGoal.FindAsync(id);

        if (goal == null)
        {
            return NotFound();
        }

        return goal;
    }

    // POST: api/Goal
    [HttpPost]
    public async Task<ActionResult<OugGoal>> CreateGoal([FromBody] GoalDto dto)
    {
        OugGoal goal = new OugGoal(dto.Name, dto.Description);

        _context.OugGoal.Add(goal);
        await _context.SaveChangesAsync();

        return await GetGoal(goal.Id);
    }


    // PATCH: api/Goal/id
    [HttpPatch("{id}")]
    public async Task<ActionResult<OugGoal>> UpdateGoal(Guid id,[FromBody] JsonElement updates)
    {

        var goal = _context.OugGoal.FirstOrDefault(g => g.Id == id);
        if (goal == null) return NotFound();

        if (updates.TryGetProperty("email", out var name))
            goal.Name = name.GetString();

        if (updates.TryGetProperty("name", out var description))
            goal.Description = description.GetString();

        return Ok(goal);
    }

    // DELETE: api/Goal/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGoal(Guid id)
    {
        var goal = await _context.OugGoal.FindAsync(id);
        if (goal == null)
        {
            return NotFound();
        }

        _context.OugGoal.Remove(goal);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
