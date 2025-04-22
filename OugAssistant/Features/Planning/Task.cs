

namespace OugAssistant.Features.Planning;

enum TaskPriority
{
    LOW, MEDIUM, HIGH
}

abstract class Task
{
    protected Guid Id { get; set; }
    protected string? Name { get; set; }
    protected string? Description { get; set; }
    protected TaskPriority Priority { get; set; }
}

