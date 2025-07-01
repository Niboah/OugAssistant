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
        // Herencia TPH (Table Per Hierarchy)
        modelBuilder.Entity<OugTask>()
            .HasDiscriminator<string>("TaskType")
            .HasValue<OugEvent>("Event")
            .HasValue<OugMission>("Mission")
            .HasValue<OugRoutine>("Routine");

        // Relación Goal - Tasks
        modelBuilder.Entity<OugGoal>()
            .HasMany(g => g.Tasks)
            .WithOne(t => t.Goal)
            .HasForeignKey(t => t.GoalId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacion Routine - Lista de WeekTimes
        modelBuilder.Entity<OugRoutine>()
            .Property(r => r.WeekTimes)
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
    private readonly PlanningDBContext _db;

    public PlanningDB(PlanningDBContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<OugTask>> GetAllOugTaskAsync(Type? type = null)
    {
        if (type == null)
            return await _db.OugTasks.Include(t=>t.Goal).ToListAsync();
        else
            return await _db.OugTasks.Include(t => t.Goal).Where(t => t.GetType() == type).ToListAsync();
    }

    public async Task<OugTask?> GetOugTaskByIdAsync(Guid id, Type? type = null)
    {
        var task = await _db.OugTasks.Include(t=>t.Goal).FirstOrDefaultAsync(t=>t.Id==id);
        if (task == null)
            return null;
        if (type == null || task.GetType() == type)
            return task;
        return null;
    }

    public async Task<bool> AddOugTaskAsync(OugTask item)
    {
        await _db.OugTasks.AddAsync(item);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateOugTaskAsync(OugTask item)
    {
        var existing = await _db.OugTasks.Include(t => t.Goal).FirstOrDefaultAsync(t => t.Id == item.Id);
        if (existing == null)
            return false;

        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.Priority = item.Priority;
        existing.GoalId = item.GoalId;
        existing.Goal = item.Goal;
        // Si necesitas actualizar propiedades específicas de subclases, hazlo aquí

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOugTaskAsync(Guid id)
    {
        var entity = await _db.OugTasks.FindAsync(id);
        if (entity != null)
        {
            _db.OugTasks.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<IEnumerable<OugGoal>> GetAllOugGoalAsync()
    {
        return await _db.OugGoal
            .Include(g => g.Tasks)
            .ToListAsync();
    }

    public async Task<OugGoal?> GetOugGoalByIdAsync(Guid id)
    {
        return await _db.OugGoal
            .Include(g => g.Tasks)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<bool> AddOugGoalAsync(OugGoal item)
    {
        await _db.OugGoal.AddAsync(item);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateOugGoalAsync(OugGoal item)
    {
        var existing = await GetOugGoalByIdAsync(item.Id);
        if (existing == null)
            return false;

        existing.Name = item.Name;
        existing.Description = item.Description;
        existing.ParentGoal = item.ParentGoal;
        existing.Archived = item.Archived;
        // Si necesitas actualizar la colección Tasks, hazlo aquí

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOugGoalAsync(Guid id)
    {
        var entity = await GetOugGoalByIdAsync(id);
        if (entity != null)
        {
            _db.OugGoal.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }
}