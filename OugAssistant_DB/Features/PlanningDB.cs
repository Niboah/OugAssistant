using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;
using OugAssistant_APP.Interfaces.IPlanningBD;

namespace OugAssistant_DB.Features;

public class PlanningDBContext : DbContext
{
    public PlanningDBContext(DbContextOptions<PlanningDBContext> options) : base(options) { }

    public DbSet<OugTask> OugTasks { get; set; }
    public DbSet<OugEvent> OugEvent { get; set; }
    public DbSet<OugMission> OugMission { get; set; }
    public DbSet<OugGoal> OugGoal { get; set; }
    public DbSet<OugRoutine> OugRoutine { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region OugGoal
        modelBuilder.Entity<OugGoal>(entity =>
        {
            entity.HasKey(g => g.Id);

            // Auto-relación (Parent - Childs)
            entity.HasOne(g => g.Parent)
                  .WithMany(g => g.Childs)
                  .HasForeignKey("ParentId")
                  .OnDelete(DeleteBehavior.Restrict);

            // Many-to-Many con OugTask (sin tabla explícita)
            entity.HasMany(g => g.Tasks)
                  .WithMany(t => t.Goals)
                  .UsingEntity<Dictionary<string, object>>(
                      "OugTaskGoal", // nombre de la tabla intermedia
                      j => j.HasOne<OugTask>()
                            .WithMany()
                            .HasForeignKey("TaskId")
                            .OnDelete(DeleteBehavior.Cascade),
                      j => j.HasOne<OugGoal>()
                            .WithMany()
                            .HasForeignKey("GoalId")
                            .OnDelete(DeleteBehavior.Cascade),
                      j =>
                      {
                          j.HasKey("TaskId", "GoalId");
                      });
        });
        #endregion

        #region OugTask
        modelBuilder.Entity<OugTask>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.HasOne(t => t.Parent)
                  .WithMany(t => t.Childs)
                  .HasForeignKey("ParentId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Table-per-hierarchy (TPH) para OugTask
        modelBuilder.Entity<OugEvent>().HasBaseType<OugTask>();
        modelBuilder.Entity<OugMission>().HasBaseType<OugTask>();
        modelBuilder.Entity<OugRoutine>().HasBaseType<OugTask>();

        // Relacion Routine - Lista de Routines
        modelBuilder.Entity<OugRoutine>()
            .Property(r => r.Routines)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                {
                    Converters = { new TimeOnlyJsonConverter() }
                }),
                v => JsonSerializer.Deserialize<HashSet<TimeOnly>[]>(v, new JsonSerializerOptions
                {
                    Converters = { new TimeOnlyJsonConverter() }
                })
            );

        #endregion
    }

    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string Format = "HH:mm";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.ParseExact(reader.GetString()!, Format);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}

public class PlanningDB : IPlanningDB
{
    private readonly PlanningDBContext _context;

    public PlanningDB(PlanningDBContext context)
    {
        _context = context;
    }

    #region OugTask

    public async Task<ICollection<OugTask>> GetOugTaskAsync()
        => await _context.OugTasks.ToListAsync();

    public async Task<ICollection<OugTask>> GetOugTaskAsync(ICollection<Guid> idList)
        => await _context.OugTasks.Where(t => idList.Contains(t.Id)).ToListAsync();

    public async Task<OugTask?> GetOugTaskByIdAsync(Guid id)
        => await _context.OugTasks.FindAsync(id);

    public async Task<bool> AddOugTaskAsync(OugTask item)
    {
        await _context.OugTasks.AddAsync(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddOugTaskAsync(ICollection<OugTask> itemList)
    {
        await _context.OugTasks.AddRangeAsync(itemList);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOugTaskAsync(OugTask item)
    {
        _context.OugTasks.Update(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOugTaskAsync(ICollection<OugTask> itemList)
    {
        _context.OugTasks.UpdateRange(itemList);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteOugTaskAsync(Guid id)
    {
        var entity = await _context.OugTasks.FindAsync(id);
        if (entity == null) return false;

        _context.OugTasks.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteOugTaskAsync(ICollection<Guid> idList)
    {
        var entities = await _context.OugTasks.Where(t => idList.Contains(t.Id)).ToListAsync();
        if (!entities.Any()) return false;

        _context.OugTasks.RemoveRange(entities);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> FinishOugTaskAsync(Guid id)
    {
        var entity = await _context.OugTasks.FindAsync(id);
        if (entity == null) return false;
        var result = entity.Finish();
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<bool> FinishOugTaskAsync(ICollection<Guid> idList)
    {
        var entities = await _context.OugTasks.Where(t => idList.Contains(t.Id)).ToListAsync();
        if (!entities.Any()) return false;
        var result = false;
        foreach (var e in entities)
            result = e.Finish() || result;

        return await _context.SaveChangesAsync() > 0;
    }

    #endregion

    #region OugGoal

    public async Task<ICollection<OugGoal>> GetOugGoalAsync()
        => await _context.OugGoal.ToListAsync();

    public async Task<ICollection<OugGoal>> GetOugGoalAsync(ICollection<Guid> idList)
        => await _context.OugGoal.Where(g => idList.Contains(g.Id)).ToListAsync();

    public async Task<OugGoal?> GetOugGoalByIdAsync(Guid id)
        => await _context.OugGoal.FindAsync(id);

    public async Task<bool> AddOugGoalAsync(OugGoal item)
    {
        await _context.OugGoal.AddAsync(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> AddOugGoalAsync(ICollection<OugGoal> itemList)
    {
        await _context.OugGoal.AddRangeAsync(itemList);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOugGoalAsync(OugGoal item)
    {
        _context.OugGoal.Update(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateOugGoalAsync(ICollection<OugGoal> itemList)
    {
        _context.OugGoal.UpdateRange(itemList);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteOugGoalAsync(Guid id)
    {
        var entity = await _context.OugGoal.FindAsync(id);
        if (entity == null) return false;

        _context.OugGoal.Remove(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteOugGoalAsync(ICollection<Guid> idList)
    {
        var entities = await _context.OugGoal.Where(g => idList.Contains(g.Id)).ToListAsync();
        if (!entities.Any()) return false;

        _context.OugGoal.RemoveRange(entities);
        return await _context.SaveChangesAsync() > 0;
    }
    #endregion
}