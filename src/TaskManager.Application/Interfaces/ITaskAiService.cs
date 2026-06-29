namespace TaskManager.Application.Interfaces;

public record AiTaskAnalysis(
    string Category,
    int EstimatedMinutes,
    string Priority,
    string AiSuggestion);

public interface ITaskAiService
{
    Task<AiTaskAnalysis> AnalyzeTaskAsync(string title, string description, CancellationToken cancellationToken = default);
}
