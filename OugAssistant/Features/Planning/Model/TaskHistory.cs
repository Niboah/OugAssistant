using Microsoft.EntityFrameworkCore;

namespace OugAssistant.Features.Planning.Model;
[Keyless]
public class TaskHistory
{
    public Task Task { get; set; }
    public DateTime FinishDateTime { get; set; }
    public string Type { get; set; } //myObject.GetType().Name;

    public TaskHistory() { }
}
