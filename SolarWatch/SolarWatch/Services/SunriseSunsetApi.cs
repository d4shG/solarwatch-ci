using System.Net;
using System.Text.Json;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class SunriseSunsetApi(ILogger<SunriseSunsetApi> logger): ISolarDataProvider
{
	private readonly ILogger<SunriseSunsetApi> _logger = logger;
	private const string BaseUrl = "https://api.sunrise-sunset.org/json?";
	
	public async Task<SolarDto> GetSolarData(CityDto location, DateTime date)
	{
		using var client = new HttpClient();

		var url = CreateUrl(location, date);

		_logger.LogInformation("Calling Sunrise-Sunset API with url: {url}", url);

		try
		{
			var jsonResponse = await client.GetStringAsync(url);
			
			return GetSolarDataFromJson(jsonResponse);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error fetching solar data from OpenWeather API.");
			throw;
		}
	}

	private static SolarDto GetSolarDataFromJson(string jsonResponse)
	{
		var jsonDocument = JsonDocument.Parse(jsonResponse);
			
		if (!jsonDocument.RootElement.TryGetProperty("results", out var resultsElement))
		{
			throw new InvalidOperationException("The 'results' field is missing from the API response.");
		}
			
		var solarData = JsonSerializer.Deserialize<SolarDto>(resultsElement.GetRawText(), new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		});
		
		return solarData ?? throw new InvalidOperationException("Failed to deserialize solar data.");
	}

	private string CreateUrl(CityDto location, DateTime date) => 
		BaseUrl + $"lat={location.Latitude}&lng={location.Latitude}&date{date:yyyy-MM-dd}";
}