using System.Net;
using System.Text.Json;
using SolarWatch.Models;

namespace SolarWatch.Services;

public class GeocodingApi(ILogger<GeocodingApi> logger): ICityDataProvider
{
	private readonly ILogger<GeocodingApi> _logger = logger;
	private const string BaseUrl = "http://api.openweathermap.org/geo/1.0/direct?";
	private const int CityLimit = 1;

	private static readonly string ApiKey =
		Environment.GetEnvironmentVariable("OpenWeatherMapApiKey")  ?? 
		throw new InvalidOperationException("API key for OpenWeatherMap is missing.");


	public async Task<CityDto> GetCityData(string cityName)
	{
		using var client = new HttpClient();

		var url = CreateUrl(cityName);

		_logger.LogInformation("Calling Geocoding API with url: {url}", url);

		try
		{
			var jsonResponse = await client.GetStringAsync(url);
			
			return GetCityDataFromJson(jsonResponse);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error fetching solar data from OpenWeather API.");
			throw;
		}
	}
	
	private static CityDto GetCityDataFromJson(string jsonResponse)
	{
		var jsonDocument = JsonDocument.Parse(jsonResponse);
		
		var cityDataArray = jsonDocument.RootElement.EnumerateArray();
		
		if (!cityDataArray.Any())
			throw new InvalidOperationException("The API response does not contain any city data.");
			
		var firstCity = cityDataArray.FirstOrDefault();

		var cityData = JsonSerializer.Deserialize<CityDto>(firstCity.GetRawText(), new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		});
		
		return cityData ?? throw new InvalidOperationException("Failed to deserialize city data.");
	}
	
	private string CreateUrl(string cityName) => 
		BaseUrl + $"q={cityName}&{CityLimit}&appid={ApiKey}";
}