using SolarWatch.Models;

namespace SolarWatch.Services;

public interface ISolarDataProvider
{
	Task<SolarDto> GetSolarData(CityDto location, DateTime date);
}