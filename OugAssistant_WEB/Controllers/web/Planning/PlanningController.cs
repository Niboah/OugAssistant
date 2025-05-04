using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OugAssistant_WEB.Models.Planning;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_WEB.Controllers.web.Planning;

public class PlanningController : Controller
{
    private readonly OugAssistant_DB.Features.Planning _context;

    public PlanningController(OugAssistant_DB.Features.Planning context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        TaskViewModel taskViewModel = new TaskViewModel();
        taskViewModel.TaskList = _context.OugTasks.ToList();
        taskViewModel.GoalList = _context.OugGoal.ToList();
        return View(taskViewModel);
    }
}