using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_APP.DTO.Planning
{
    public class GoalAPIout
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Archived { get; set; }
        public int Level { get; set; }
        public GoalAPIout ParentGoal { get; set; }
        public ICollection<GoalAPIout> ChildGoals { get; set; }
        public ICollection<TaskAPIout> Tasks { get; set; }

        public GoalAPIout(OugGoal goal)
        {
            Id = goal.Id;
            Name = goal.Name;
            Description = goal.Description;
            Archived = goal.Archived;
            Level = goal.Level;
            ParentGoal = goal.ParentGoal != null ? new GoalAPIout(goal.ParentGoal) : null;
            ChildGoals = goal.ChildGoals.Select(g => new GoalAPIout(g)).ToList();
            Tasks = goal.Tasks.Select(t => new TaskAPIout(t)).ToList();
        }
    }
    public class GoalAPIin
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid? ParentGoalId { get; set; }
    }

}
