using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using OugAssistant_APP.Interfaces.IPlanningBD;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.DTO.Planning;
using OugAssistant_APP.Sevices.Planning;


namespace Tests
{
    public class GoalServicesTests
    {
        [Fact]
        public async Task GetAllOugGoalAsync_ReturnsOrderedGoals()
        {
            var mockDb = new Mock<IPlanningDB>();
            var goals = new List<OugGoal>
        {
            new OugGoal("A", "DescA"),
            new OugGoal("B", "DescB") 
        };
            mockDb.Setup(db => db.GetAllOugGoalAsync()).ReturnsAsync(goals);

            var service = new GoalServices(mockDb.Object);

            var result = await service.GetAllOugGoalAsync();

            Assert.Equal(2, result.Count());
            Assert.Equal("A", result.First().Name); // Ordered by Level descending
        }

        [Fact]
        public async Task AddOugGoalAsync_CallsDbAndReturnsTrue()
        {
            var mockDb = new Mock<IPlanningDB>();
            mockDb.Setup(db => db.AddOugGoalAsync(It.IsAny<OugGoal>())).ReturnsAsync(true);

            var service = new GoalServices(mockDb.Object);

            var input = new GoalAPIin { Name = "Test", Description = "Desc" };
            var result = await service.AddOugGoalAsync(input);

            Assert.True(result);
        }
    }
}
