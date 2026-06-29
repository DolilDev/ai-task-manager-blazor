# AI Task Manager

A Blazor Server task management application built with .NET 9 and Clean Architecture. Create tasks and let NVIDIA AI (Llama 3.1) automatically categorize, estimate time, set priority, and suggest next steps.

## Architecture

```
TaskManager.sln
├── TaskManager.Domain          # Entities and enums
├── TaskManager.Application     # Use cases, interfaces, DTOs
├── TaskManager.Infrastructure  # EF Core (SQLite), NVIDIA NIM API
└── TaskManager.Web             # Blazor Server UI
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [NVIDIA API Key](https://build.nvidia.com/) (free tier available)

## Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd ai-task-manager-blazor
   ```

2. **Configure NVIDIA API key**

   Edit `src/TaskManager.Web/appsettings.json` and replace the placeholder:

   ```json
   "NvidiaAI": {
     "ApiKey": "YOUR_KEY_HERE"
   }
   ```

   Get your API key from [NVIDIA Build](https://build.nvidia.com/).

3. **Restore and run**

   ```bash
   dotnet restore
   dotnet run --project src/TaskManager.Web
   ```

4. **Open in browser**

   Navigate to `https://localhost:7299` (or the URL shown in the terminal).

## Features

- **Add tasks** with title and description
- **AI analysis** on creation — category, estimated minutes, priority, and actionable suggestion
- **Task list** with status badges and filter buttons (All / Todo / InProgress / Done)
- **Update status** via dropdown on each task row
- **SQLite database** — created automatically on first run (`taskmanager.db`)

## NVIDIA NIM Integration

The app uses the NVIDIA NIM OpenAI-compatible API:

- **Endpoint:** `https://integrate.api.nvidia.com/v1`
- **Model:** `meta/llama-3.1-8b-instruct`

If no API key is configured, the app falls back to default values so you can still test the UI.

## Project Structure Details

| Layer | Responsibility |
|-------|----------------|
| **Domain** | `TaskItem` entity, `TaskPriority` and `TaskStatus` enums |
| **Application** | `CreateTaskUseCase`, `GetAllTasksUseCase`, `UpdateTaskStatusUseCase` |
| **Infrastructure** | `TaskRepository`, `NvidiaAiService`, EF Core `AppDbContext` |
| **Web** | Blazor Server components: `AddTask`, `TaskList`, filter UI |
