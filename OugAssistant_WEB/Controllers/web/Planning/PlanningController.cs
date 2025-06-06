using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OugAssistant_WEB.Models.Planning;
using OugAssistant.Features.Planning.Model;
using System.Diagnostics;
using System.Text.Json;

namespace OugAssistant_WEB.Controllers.web.Planning;

public class PlanningController : Controller
{
    private readonly OugAssistant_DB.Features.Planning _context;
    private readonly ILogger _logger;

    public PlanningController(OugAssistant_DB.Features.Planning context, ILogger<PlanningController> logger)
    {
        _context = context;
        _logger = logger;
        _logger.LogDebug(1, "NLog injected into PlanningController");
    }

    public IActionResult Index()
    {
        _logger.LogInformation("PlanningController Index");

        TaskViewModel taskViewModel = new TaskViewModel();
        taskViewModel.TaskList = _context.OugTasks.Where(x=>x.FinishDateTime==null).ToList();
        _logger.LogInformation("TaskList " + JsonSerializer.Serialize(taskViewModel.TaskList) );
        taskViewModel.GoalList = _context.OugGoal.ToList();
        _logger.LogInformation("GoalList " + JsonSerializer.Serialize(taskViewModel.GoalList));
        return View(taskViewModel);
    }

    public IActionResult TaskList() {
        _logger.LogInformation("PlanningController Index");

        TaskViewModel taskViewModel = new TaskViewModel();
        taskViewModel.TaskList = _context.OugTasks.Where(x => x.FinishDateTime == null).ToList();
        _logger.LogInformation("TaskList " + JsonSerializer.Serialize(taskViewModel.TaskList));
        taskViewModel.GoalList = _context.OugGoal.ToList();
        _logger.LogInformation("GoalList " + JsonSerializer.Serialize(taskViewModel.GoalList));
        return PartialView("_taskList", taskViewModel);
    }
}