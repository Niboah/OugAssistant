using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;


namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class RoutineController : ControllerBase
{

    private readonly OugAssistant_DB.Features.Planning _context;

    public RoutineController(OugAssistant_DB.Features.Planning context)
    {
        _context = context;
    }

    public class RoutineDto
    {
        public required List<DayOfWeek> WeekDay { get; set; }
        public required List<TimeSpan> TimeDay { get; set; }
    }

    // GET: api/Routine
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OugRoutine>>> GetRoutines()
    {
        return await _context.OugRoutine
            .ToListAsync();
    }

    // GET: api/Routine/id
    [HttpGet("{id}")]
    public async Task<ActionResult<OugRoutine>> GetRoutine(Guid id)
    {
        var routine = await _context.OugRoutine.FindAsync(id);

        if (routine == null)
        {
            return NotFound();
        }

        return routine;
    }

    // POST: api/Routine
    [HttpPost]
    public async Task<ActionResult<OugRoutine>> CreateRoutine([FromBody] RoutineDto dto)
    {
        OugRoutine routine = new OugRoutine(dto.WeekDay, dto.TimeDay);

        _context.OugRoutine.Add(routine);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetRoutine", new { id = routine.Id }, routine);
    }

    // DELETE: api/Routine/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoutine(Guid id)
    {
        OugRoutine routine = await _context.OugRoutine.FindAsync(id);
        if (routine == null)
        {
            return NotFound();
        }

        _context.OugRoutine.Remove(routine);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
