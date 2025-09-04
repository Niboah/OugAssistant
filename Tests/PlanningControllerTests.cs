using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using OugAssistant_WEB.Controllers.web.Planning;
using OugAssistant_APP.Interfaces.Planning;
using OugAssistant_WEB.Models.Planning;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OugAssistant_APP.DTO.Planning;

namespace Tests
{
    public class PlanningControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithTaskViewModel()
        {
            // Arrange
            var mockTaskServices = new Mock<ITaskServices>();
            var mockGoalServices = new Mock<IGoalServices>();
            var mockLogger = new Mock<ILogger<PlanningController>>();

            mockTaskServices.Setup(s => s.GetAllOugTaskAsync(false, null))
                .ReturnsAsync(new List<TaskAPIout>());
            mockGoalServices.Setup(s => s.GetAllOugGoalAsync())
                .ReturnsAsync(new List<GoalAPIout>());

            var controller = new PlanningController(mockTaskServices.Object, mockGoalServices.Object, mockLogger.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PlanningViewModel>(viewResult.Model);
            Assert.NotNull(model.TaskList);
            Assert.NotNull(model.GoalList);
        }

        [Fact]
        public async Task TaskList_ReturnsPartialViewWithTaskViewModel()
        {
            // Arrange
            var mockTaskServices = new Mock<ITaskServices>();
            var mockGoalServices = new Mock<IGoalServices>();
            var mockLogger = new Mock<ILogger<PlanningController>>();

            mockTaskServices.Setup(s => s.GetAllOugTaskAsync(false, null))
                .ReturnsAsync(new List<TaskAPIout>());
            mockGoalServices.Setup(s => s.GetAllOugGoalAsync())
                .ReturnsAsync(new List<GoalAPIout>());

            var controller = new PlanningController(mockTaskServices.Object, mockGoalServices.Object, mockLogger.Object);

            // Act
            var result = await controller.TaskList();

            // Assert
            var partialResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsType<PlanningViewModel>(partialResult.Model);
            Assert.NotNull(model.TaskList);
            Assert.NotNull(model.GoalList);
        }
    }
}
