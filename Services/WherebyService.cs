using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace InternconnectBackend.Services
{
	public class WherebyService
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		private readonly ILogger<WherebyService> _logger;

		public WherebyService(HttpClient httpClient, IConfiguration configuration, ILogger<WherebyService> logger)
		{
			_httpClient = httpClient;
			_apiKey = configuration["Whereby:ApiKey"] ?? throw new ArgumentNullException("Whereby API Key is missing");
			_logger = logger;
		}

		public async Task<string?> CreateMeetingAsync(DateTime date, TimeSpan startTime, TimeSpan endTime)
		{
			try
			{
				var requestBody = new
				{
					startDate = date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
					endDate = date.Add(startTime).ToString("yyyy-MM-ddTHH:mm:ssZ"),
					roomNamePrefix = $"meeting",
					roomMode = "normal"
				};

				var requestJson = JsonSerializer.Serialize(requestBody);
				var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

				// Tambahkan API Key ke Header Authorization
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

				var response = await _httpClient.PostAsync("https://api.whereby.dev/v1/meetings", content);

				_logger.LogInformation($"Using Whereby API Key: {_apiKey}");


				var responseContent = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError($"Whereby API error: {response.StatusCode} - {responseContent}");
					return null;
				}

				var meetingResponse = JsonSerializer.Deserialize<Models.WherebyMeetingResponse>(responseContent);
				if (meetingResponse == null || string.IsNullOrEmpty(meetingResponse.RoomUrl))
				{
					_logger.LogError($"Whereby response invalid: {responseContent}");
					return null;
				}

				_logger.LogInformation($"Meeting berhasil dibuat: {meetingResponse.RoomUrl}");
				return meetingResponse.RoomUrl;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Exception saat membuat meeting Whereby: {ex.Message}");
				return null;
			}
		}

	}

}