using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Options;

namespace TaskManager.Infrastructure.Services;

public class NvidiaAiService : ITaskAiService
{
    private const string ApiUrl = "https://integrate.api.nvidia.com/v1/chat/completions";
    private const string Model = "meta/llama-3.1-8b-instruct";

    private readonly HttpClient _httpClient;
    private readonly NvidiaAiOptions _options;
    private readonly ILogger<NvidiaAiService> _logger;

    public NvidiaAiService(
        HttpClient httpClient,
        IOptions<NvidiaAiOptions> options,
        ILogger<NvidiaAiService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<AiTaskAnalysis> AnalyzeTaskAsync(
        string title,
        string description,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || _options.ApiKey == "YOUR_KEY_HERE")
        {
            _logger.LogWarning("NVIDIA API key not configured, using fallback analysis");
            return CreateFallbackAnalysis(title, description);
        }

        try
        {
            var prompt = $"""
                Analyze this task and respond with ONLY valid JSON (no markdown, no explanation):
                {{
                  "category": "one of: Work, Personal, Health, Learning, Finance, Other",
                  "estimatedMinutes": <integer estimate>,
                  "priority": "one of: Low, Medium, High, Critical",
                  "aiSuggestion": "brief actionable suggestion for completing this task"
                }}

                Title: {title}
                Description: {description}
                """;

            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = "You are a task management assistant. Always respond with valid JSON only." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                max_tokens = 512
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(responseJson);

            var content = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("Empty AI response, using fallback");
                return CreateFallbackAnalysis(title, description);
            }

            var jsonContent = ExtractJson(content);
            var analysis = JsonSerializer.Deserialize<AiAnalysisResult>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (analysis is null)
            {
                return CreateFallbackAnalysis(title, description);
            }

            return new AiTaskAnalysis(
                analysis.Category ?? "Other",
                analysis.EstimatedMinutes > 0 ? analysis.EstimatedMinutes : 30,
                analysis.Priority ?? "Medium",
                analysis.AiSuggestion ?? "Break this task into smaller steps.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze task with NVIDIA AI");
            return CreateFallbackAnalysis(title, description);
        }
    }

    private static string ExtractJson(string content)
    {
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');
        if (start >= 0 && end > start)
            return content[start..(end + 1)];
        return content;
    }

    private static AiTaskAnalysis CreateFallbackAnalysis(string title, string description) =>
        new(
            Category: "Other",
            EstimatedMinutes: 30,
            Priority: "Medium",
            AiSuggestion: $"Review and plan: {title}. {(string.IsNullOrWhiteSpace(description) ? "Add more details to get better AI suggestions." : description)}");

    private sealed class ChatCompletionResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    private sealed class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    private sealed class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }

    private sealed class AiAnalysisResult
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("estimatedMinutes")]
        public int EstimatedMinutes { get; set; }

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("aiSuggestion")]
        public string? AiSuggestion { get; set; }
    }
}
