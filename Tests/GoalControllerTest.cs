using Xunit;
using Moq;
using OugAssistant_APP.Sevices.Planning;
using OugAssistant_APP.Interfaces.IPlanningBD;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using System.Collections.Generic;
using System.Threading.Tasks;
using OugAssistant_APP.Interfaces.Planning;
using Microsoft.AspNetCore.Mvc;
using OugAssistant_WEB.Controllers.api.Planning;

namespace Tests
{
    public class GoalControllerTest
    {
        [Fact]
        public async Task GetGoals_ReturnsOkWithGoals()
        {
            var mockGoalServices = new Mock<IGoalServices>();
            mockGoalServices.Setup(s => s.GetAllOugGoalAsync())
                .ReturnsAsync(new List<GoalAPIout>());

            var controller = new GoalController(mockGoalServices.Object);

            var result = await controller.GetGoals();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<GoalAPIout>>(okResult.Value);
        }

        [Fact]
        public async Task GetGoal_ReturnsOk_WhenGoalExists()
        {
            var mockGoalServices = new Mock<IGoalServices>();
            var goal = new GoalAPIout(new OugGoal("Test", "Desc"));
            mockGoalServices.Setup(s => s.GetOugGoalByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(goal);

            var controller = new GoalController(mockGoalServices.Object);

            var result = await controller.GetGoal(Guid.NewGuid());

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<GoalAPIout>(okResult.Value);
        }

        [Fact]
        public async Task GetGoal_ReturnsNotFound_WhenGoalDoesNotExist()
        {
            var mockGoalServices = new Mock<IGoalServices>();
            mockGoalServices.Setup(s => s.GetOugGoalByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((GoalAPIout)null);

            var controller = new GoalController(mockGoalServices.Object);

            var result = await controller.GetGoal(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}