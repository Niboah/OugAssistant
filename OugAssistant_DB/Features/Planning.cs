using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_DB.Features;

public class Planning : DbContext
{
    public Planning(DbContextOptions<Planning> options) : base(options) { }

    public DbSet<OugTask> OugTasks { get; set; }
    public DbSet<OugEvent> OugEvent { get; set; }
    public DbSet<OugMission> OugMission { get; set; }
    public DbSet<OugRoutineTask> OugRoutineTask { get; set; }
    public DbSet<OugGoal> OugGoal { get; set; }
    public DbSet<OugRoutine> OugRoutine { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Herencia TPH (Table Per Hierarchy)
        modelBuilder.Entity<OugTask>()
            .HasDiscriminator<string>("TaskType")
            .HasValue<OugEvent>("Event")
            .HasValue<OugMission>("Mission")
            .HasValue<OugRoutineTask>("RoutineTask");

        // Relación Goal - Tasks
        modelBuilder.Entity<OugGoal>()
            .HasMany(g => g.Tasks)
            .WithOne(t => t.Goal)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Routine - RoutineTask
        modelBuilder.Entity<OugRoutineTask>()
            .HasOne(rt => rt.Routine)
            .WithMany() // No colección inversa en Routine
            .HasForeignKey(rt => rt.RoutineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacion Routine - Lista de weekdays
        modelBuilder.Entity<OugRoutine>()
            .Property(r => r.WeekDay)
            .HasConversion(
                v => string.Join(',', v.Select(d => (int)d)), // Día -> int -> string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(x => (DayOfWeek)int.Parse(x))
                      .ToList()
            );

        // Relacion Routine - Lista de timedays
        modelBuilder.Entity<OugRoutine>()
            .Property(r => r.TimeDay)
            .HasConversion(
                v => string.Join(',', v.Select(t => t.ToString())), // TimeSpan -> string
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(TimeSpan.Parse)
                      .ToList()
            );
    }
}
