using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace OugAssistant_WEB.Controllers.api.Planning;

[Route("api/[controller]")]
[ApiController]
public class GoalController : ControllerBase
{

    private readonly OugAssistant_APP.Interfaces.Planning.IGoalServices _goalServices;
    public GoalController(OugAssistant_APP.Interfaces.Planning.IGoalServices goalServices)
    {
        _goalServices = goalServices;
    }

    // GET: api/Goal
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OugGoal>>> GetGoals()
    {
        var result = await _goalServices.GetAllOugGoalAsync();
        return Ok(result);
    }

    // GET: api/Goal/id
    [HttpGet("{id}")]
    public async Task<ActionResult<OugGoal>> GetGoal(Guid id)
    {
        var goal = await _goalServices.GetOugGoalByIdAsync(id);

        if (goal == null)
        {
            return NotFound();
        }

        return Ok(goal);
    }

    // POST: api/Goal
    [HttpPost]
    public async Task<ActionResult<OugGoal>> CreateGoal([FromBody] GoalAPIin goal)
    {
        await _goalServices.AddOugGoalAsync(goal);
        return await GetGoal(goal.Id);
    }
    
    // POST: api/Goal/Finish/id
    [HttpPost("Finish/{id}")]
    public async Task<ActionResult<bool>> FinishGoal(Guid id)
    {
       return await _goalServices.FinishGoal(id);
    }

    // PATCH: api/Goal/id
    [HttpPatch("{id}")]
    public async Task<ActionResult<OugGoal>> UpdateGoal(Guid id, [FromBody] GoalAPIin goal)
    {
        if (id != goal.Id) return BadRequest();
        await _goalServices.UpdateOugGoalAsync(goal);
        return Ok(goal);
    }

    // DELETE: api/Goal/id
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteGoal(Guid id)
    {
        return BadRequest();
        return Ok(await _goalServices.DeleteOugGoalAsync(id));
    }
}
