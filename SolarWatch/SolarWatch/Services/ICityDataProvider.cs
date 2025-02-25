using SolarWatch.Models;

namespace SolarWatch.Services;

public interface ICityDataProvider
{
	Task<CityDto> GetCityData(string cityName);
}