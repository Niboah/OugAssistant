using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OugAssistant_WEB.Models.Planning;
using OugAssistant.Features.Planning.Model;
using System.Diagnostics;
using System.Text.Json;
using OugAssistant_APP.Interfaces.Planning;
using OugAssistant_APP.Sevices.Planning;

namespace OugAssistant_WEB.Controllers.web.Planning;

public class PlanningController : Controller
{
    private readonly ITaskServices _taskServices;
    private readonly IGoalServices _goalServices;
    private readonly ILogger _logger;

    public PlanningController(ITaskServices taskServices, IGoalServices goalServices, ILogger<PlanningController> logger)
    {
        _taskServices = taskServices;
        _goalServices = goalServices;
        _logger = logger;
        _logger.LogDebug(1, "NLog injected into PlanningController");
    }

    public async Task<IActionResult> Index()
    {
        _logger.LogInformation("PlanningController Index");

        PlanningViewModel taskViewModel = new PlanningViewModel();
        
        taskViewModel.TaskList = await _taskServices.GetAllOugTaskAsync();
        taskViewModel.GoalList = await _goalServices.GetAllOugGoalAsync();
        return View(taskViewModel);
    }

    public async Task<IActionResult> TaskList() {
        _logger.LogInformation("PlanningController TaskList");
        PlanningViewModel taskViewModel = new PlanningViewModel();
        taskViewModel.TaskList = await _taskServices.GetAllOugTaskAsync();
        taskViewModel.GoalList = await _goalServices.GetAllOugGoalAsync();
        return PartialView("Task/Task", taskViewModel);
    }

    public async Task<IActionResult> GoalList()
    {
        _logger.LogInformation("PlanningController GoalList");
        PlanningViewModel goalViewModel = new PlanningViewModel();
        goalViewModel.TaskList = await _taskServices.GetAllOugTaskAsync();
        goalViewModel.GoalList = await _goalServices.GetAllOugGoalAsync();
        return PartialView("Goal/Goal", goalViewModel);
    }
}