using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OugAssistant.Features.Planning.Model;

namespace OugAssistant_DB.Features;

public class Planning : DbContext
{
    public Planning(DbContextOptions<Planning> options) : base(options) { }

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
