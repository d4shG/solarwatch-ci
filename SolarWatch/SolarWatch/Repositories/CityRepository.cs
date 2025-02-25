using System.Data;
using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;

namespace SolarWatch.Repositories;

public class CityRepository(SolarWatchContext solarWatchContext): ICityRepository
{
	private readonly SolarWatchContext _dbContext = solarWatchContext;

	public async Task<IEnumerable<City>> GetAll() => 
		await _dbContext.Cities.ToListAsync();
	

	public async Task<City?> GetByName(string name) =>
		 await _dbContext.Cities.FirstOrDefaultAsync(c => c.Name == name);
	


	public async Task<int> Add(City city)
	{
		try
		{
			await _dbContext.AddAsync(city);
			await _dbContext.SaveChangesAsync();
			
			return city.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new InvalidDataException(e.Message);
		}
	}

	public async Task<bool> Delete(City city)
	{
		try
		{
			_dbContext.Remove(city);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new InvalidDataException(e.Message);
		}
	}

	public async Task<bool> Update(City city)
	{  
		try
		{
			_dbContext.Update(city);
			await _dbContext.SaveChangesAsync();
			
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new InvalidDataException(e.Message);
		}
	}
}