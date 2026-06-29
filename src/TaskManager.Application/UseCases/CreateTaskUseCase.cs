using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.UseCases;

public class CreateTaskUseCase
{
    private readonly ITaskRepository _repository;
    private readonly ITaskAiService _aiService;

    public CreateTaskUseCase(ITaskRepository repository, ITaskAiService aiService)
    {
        _repository = repository;
        _aiService = aiService;
    }

    public async Task<TaskDto> ExecuteAsync(string title, string description, CancellationToken cancellationToken = default)
    {
        var analysis = await _aiService.AnalyzeTaskAsync(title, description, cancellationToken);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Category = analysis.Category,
            EstimatedMinutes = analysis.EstimatedMinutes,
            Priority = ParsePriority(analysis.Priority),
            Status = TaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            AiSuggestion = analysis.AiSuggestion
        };

        var created = await _repository.AddAsync(task, cancellationToken);
        return MapToDto(created);
    }

    private static TaskPriority ParsePriority(string priority) =>
        Enum.TryParse<TaskPriority>(priority, ignoreCase: true, out var result)
            ? result
            : TaskPriority.Medium;

    private static TaskDto MapToDto(TaskItem task) =>
        new(
            task.Id,
            task.Title,
            task.Description,
            task.Category,
            task.EstimatedMinutes,
            task.Priority,
            task.Status,
            task.CreatedAt,
            task.AiSuggestion);
}
