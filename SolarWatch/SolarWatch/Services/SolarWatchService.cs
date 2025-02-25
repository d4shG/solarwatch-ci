using System.Security.Authentication;
using SolarWatch.Models;
using SolarWatch.Repositories;

namespace SolarWatch.Services;

public class SolarWatchService(
	ISolarDataProvider solarDataProvider,
	ICityDataProvider cityDataProvider,
	ISolarRepository solarRepository,
	ICityRepository cityRepository)
{
	private readonly ISolarDataProvider _solarDataProvider = solarDataProvider;
	private readonly ICityDataProvider _cityDataProvider = cityDataProvider;
	private readonly ISolarRepository _solarRepository = solarRepository;
	private readonly ICityRepository _cityRepository = cityRepository;

	public async Task<IEnumerable<string>> GetAllCities()
	{
		var cities = await _cityRepository.GetAll();
		
		return cities.Select(city => city.Name);
	}

	public async Task<SolarDto> GetSolarInfo(string cityName, DateTime date, bool isAdmin)
	{
		var city = await _cityRepository.GetByName(cityName);

		if (city is not null)
			return await GetSolarData(city, date);

		return isAdmin ? await GetSolarData(cityName, date) : throw new AuthenticationException("Not an admin");
	}

	private async Task<SolarDto> GetSolarData(City city, DateTime date)
	{
		
		var solar = await _solarRepository.GetSolar(city.Id, date);

		if (solar is not null)
			return new SolarDto()
			{
				Sunrise = solar.Sunrise,
				Sunset = solar.Sunset,
			};


		var location = new CityDto
		{
			Latitude = city.Latitude,
			Longitude = city.Longitude,
		};
		
		var result = await _solarDataProvider.GetSolarData(location, date);
		
		await _solarRepository.Add(new Solar()
		{
			CityId = city.Id,
			Sunset = result.Sunset,
			Sunrise = result.Sunrise,
			Date = date
		});
		
		return result;
	}

	private async Task<SolarDto> GetSolarData(string cityName, DateTime date)
	{
		var location = await _cityDataProvider.GetCityData(cityName);
		var cityId = await _cityRepository.Add(new City()
		{
			Name = cityName,
			Latitude = location.Latitude,
			Longitude = location.Longitude,
		});

		var result = await _solarDataProvider.GetSolarData(location, date);

		var newSolar = new Solar()
		{
			CityId = cityId,
			Sunset = result.Sunset,
			Sunrise = result.Sunrise,
			Date = date
		};
		
		await _solarRepository.Add(newSolar);
		
		return result;
	}
}