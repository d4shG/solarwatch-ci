using SolarWatch.Models;

namespace SolarWatch.Repositories;

public interface ICityRepository
{
	Task<IEnumerable<City>> GetAll();
	Task<City?> GetByName(string name);
	Task<int> Add(City city);
	Task<bool> Delete(City city);
	Task<bool> Update(City city);
}