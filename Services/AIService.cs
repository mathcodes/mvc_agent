
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AI_Agent_MVC_App.Services;

public class AIService
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;

	public AIService(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_configuration = configuration;
	}

	public async Task<string> GetResponseAsync(string prompt, CancellationToken cancellationToken = default)
	{
		var apiKey = _configuration["OPENAI_API_KEY"];
		if (string.IsNullOrWhiteSpace(apiKey))
		{
			throw new InvalidOperationException(
				"The OPENAI_API_KEY environment variable is not set. Set it and restart the app.");
		}

		var model = _configuration["OpenAI:Model"] ?? "gpt-4o-mini";
		using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
		request.Content = new StringContent(
			JsonSerializer.Serialize(new
			{
				model,
				messages = new[]
				{
					new { role = "user", content = prompt }
				}
			}),
			Encoding.UTF8,
			"application/json");

		using var response = await _httpClient.SendAsync(request, cancellationToken);
		var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			var detail = TryGetApiError(responseBody);
			throw new InvalidOperationException(
				detail ?? $"The AI provider returned HTTP {(int)response.StatusCode} ({response.ReasonPhrase}).");
		}

		using var document = JsonDocument.Parse(responseBody);
		if (document.RootElement.TryGetProperty("choices", out var choices) &&
			choices.GetArrayLength() > 0 &&
			choices[0].TryGetProperty("message", out var message) &&
			message.TryGetProperty("content", out var content) &&
			content.ValueKind == JsonValueKind.String)
		{
			return content.GetString() ?? string.Empty;
		}

		throw new InvalidOperationException("The AI provider returned a response in an unexpected format.");
	}

	private static string? TryGetApiError(string responseBody)
	{
		try
		{
			using var document = JsonDocument.Parse(responseBody);
			if (document.RootElement.TryGetProperty("error", out var error) &&
				error.TryGetProperty("message", out var message))
			{
				return message.GetString();
			}
		}
		catch (JsonException)
		{
		}

		return null;
	}
}
