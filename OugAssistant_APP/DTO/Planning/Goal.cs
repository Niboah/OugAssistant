using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int Level { get; set; }
        public float Priority { get; set; }
        public GoalAPIout? ParentGoal { get; set; }
        public ICollection<GoalAPIout> ChildGoals { get; set; }
        public ICollection<TaskAPIout> Tasks { get; set; }

        public GoalAPIout(OugGoal goal, Dictionary<Guid, Object>? visited = null)
        {
            visited ??= new Dictionary<Guid, Object>();
            bool first = visited.Count() == 0;

            if (visited.ContainsKey(goal.Id))
            {
                // Ya está registrado, usar la instancia existente.
                var existing = (GoalAPIout)visited[goal.Id];
                Id = existing.Id;
                Name = existing.Name;
                Description = existing.Description;
                Level = existing.Level;
                
                ParentGoal = existing.ParentGoal;
                ChildGoals = existing.ChildGoals;
                Tasks = existing.Tasks;

                Priority = (1 / existing.Level) * (ParentGoal != null ? ParentGoal.Priority : 1);
                return;
            }

            // Agregar primero para prevenir ciclos
            visited.Add(goal.Id, this);

            Id = goal.Id;
            Name = goal.Name;
            Description = goal.Description;
            Level = goal.Level;
            

            ParentGoal = goal.Parent != null
                ? (visited.ContainsKey(goal.Parent.Id)
                    ? null//(GoalAPIout)visited[goal.ParentGoal.Id]
                    : new GoalAPIout(goal.Parent, visited))
                : null;

            ChildGoals = goal.Childs?
                .Select(g => visited.ContainsKey(g.Id)
                    ? (first ? (GoalAPIout)visited[g.Id] : null) 
                    : new GoalAPIout(g, visited))
                .ToList()
                ?? new List<GoalAPIout>();

            Tasks = goal.Tasks?
                .Select(t => visited.ContainsKey(t.Id)
                    ? (first ? (TaskAPIout)visited[t.Id] : null)
                    : new TaskAPIout(t, true, visited))
                .ToList()
                ?? new List<TaskAPIout>();


            Priority = (goal.Level == 0 ? 1 :  (1 / goal.Level)) * (ParentGoal!=null ? ParentGoal.Priority : 1);
        }
    }
    public class GoalAPIin
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "El campo Name es obligatorio")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentGoalId { get; set; }
    }

}
