namespace TaskManager.Infrastructure.Options;

public class NvidiaAiOptions
{
    public const string SectionName = "NvidiaAI";

    public string ApiKey { get; set; } = string.Empty;
}
