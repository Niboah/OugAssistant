
namespace OugAssistant.Features.Planning;

internal class TaskHistory
{
    Task Task { get; set; }
    DateTime FinishDateTime { get; set; }
    string Type { get; set; } //myObject.GetType().Name;
}
