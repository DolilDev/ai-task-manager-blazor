using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Application.UseCases;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Options;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NvidiaAiOptions>(configuration.GetSection(NvidiaAiOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=taskmanager.db"));

        services.AddScoped<ITaskRepository, TaskRepository>();

        services.AddHttpClient<ITaskAiService, NvidiaAiService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateTaskUseCase>();
        services.AddScoped<GetAllTasksUseCase>();
        services.AddScoped<UpdateTaskStatusUseCase>();

        return services;
    }

    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}
