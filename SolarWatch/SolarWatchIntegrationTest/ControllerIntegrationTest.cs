using System.Net.Http.Headers;
using System.Net.Http.Json;
using SolarWatch.Models;
using SolarWatch.Services.Authentication;
using SolarWatchTest.IntegrationTest;

namespace SolarWatchIntegrationTest;

[Collection("IntegrationTests")]
public class ControllerIntegrationTest
{
	private readonly SolarWatchWebApplicationFactory _app;
	private readonly HttpClient _client;
    
	public ControllerIntegrationTest()
	{
		_app = new SolarWatchWebApplicationFactory();
		_client = _app.CreateClient();
	}
	
	[Fact]
	public async Task TestEndPoint()
	{
		var city = "London"; 
		var date = "2025-02-25";
		
		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", "mock-jwt-token");
		
		var response = await _client.GetAsync($"https://localhost:8080/solar-watch?city={city}&date={date}");


		var responseBody = await response.Content.ReadAsStringAsync();
		Assert.True(response.IsSuccessStatusCode, 
			$"Error: {response.StatusCode}, Response Body: {responseBody}");

		var data = await response.Content.ReadFromJsonAsync<SolarDto>();
		Assert.NotNull(data);

		Assert.Contains("AM", data?.Sunrise);
		Assert.Contains("PM", data?.Sunset);
	}
	
	[Fact]
	public async Task TestInvalidCity()
	{
		var city = "InvalidCity";
		var date = "2025-02-25";

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", "mock-jwt-token");

		var response = await _client.GetAsync($"https://localhost:8080/solar-watch?city={city}&date={date}");

		var responseBody = await response.Content.ReadAsStringAsync();
		Assert.False(response.IsSuccessStatusCode, 
			$"Error: {response.StatusCode}, Response Body: {responseBody}");

		Assert.Contains("The API response does not contain any city", responseBody);
	}
	
	[Fact]
	public async Task TestInvalidDateFormat()
	{
		var city = "London";
		var date = "25-02-2025";

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", "mock-jwt-token");

		var response = await _client.GetAsync($"https://localhost:8080/solar-watch?city={city}&date={date}");

		var responseBody = await response.Content.ReadAsStringAsync();
		Assert.False(response.IsSuccessStatusCode, 
			$"Error: {response.StatusCode}, Response Body: {responseBody}");

		Assert.Contains($"The value '{date}' is not valid", responseBody);
	}
	
	[Fact]
	public async Task TestMissingCityParameter()
	{
		var date = "2025-02-25"; 

		_client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", "mock-jwt-token");

		var response = await _client.GetAsync($"https://localhost:8080/solar-watch?date={date}");

		var responseBody = await response.Content.ReadAsStringAsync();
		Assert.False(response.IsSuccessStatusCode, 
			$"Error: {response.StatusCode}, Response Body: {responseBody}");

		Assert.Contains("The city field is required.", responseBody);
	}


}