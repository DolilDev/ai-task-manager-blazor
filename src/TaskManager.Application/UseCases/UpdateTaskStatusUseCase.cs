using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.UseCases;

public class UpdateTaskStatusUseCase
{
    private readonly ITaskRepository _repository;

    public UpdateTaskStatusUseCase(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskDto?> ExecuteAsync(Guid id, TaskStatus status, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
            return null;

        task.Status = status;
        await _repository.UpdateAsync(task, cancellationToken);
        return MapToDto(task);
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
