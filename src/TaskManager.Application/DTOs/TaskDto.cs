using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string Description,
    string Category,
    int EstimatedMinutes,
    TaskPriority Priority,
    TaskStatus Status,
    DateTime CreatedAt,
    string AiSuggestion);
