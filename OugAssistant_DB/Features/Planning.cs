using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_DB.Features;

public class Planning : DbContext
{
    public Planning(DbContextOptions<Planning> options) : base(options) { }

    public DbSet<Goal> Goals { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Mission> Missions { get; set; }
    public DbSet<Routine> Routines { get; set; }
    public DbSet<RoutineTask> RoutineTasks { get; set; }
    public DbSet<TaskHistory> TaskHistory { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Goal>().ToTable("Goal");
        modelBuilder.Entity<Event>().ToTable("Event");
        modelBuilder.Entity<Mission>().ToTable("Mission");
        modelBuilder.Entity<Routine>().ToTable("Routine");
        modelBuilder.Entity<RoutineTask>().ToTable("RoutineTask");
        modelBuilder.Entity<TaskHistory>().ToTable("TaskHistory");
    }
}
