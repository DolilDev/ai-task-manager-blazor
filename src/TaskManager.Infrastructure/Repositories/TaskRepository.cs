using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.FindAsync([id], cancellationToken);
    }

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task;
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
