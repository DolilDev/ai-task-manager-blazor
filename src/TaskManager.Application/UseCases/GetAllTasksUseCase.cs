using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.UseCases;

public class GetAllTasksUseCase
{
    private readonly ITaskRepository _repository;

    public GetAllTasksUseCase(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TaskDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await _repository.GetAllAsync(cancellationToken);
        return tasks.Select(MapToDto).ToList();
    }

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
