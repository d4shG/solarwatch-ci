using Microsoft.EntityFrameworkCore;
using SolarWatch.Models;

namespace SolarWatch.Repositories;

public class SolarRepository(SolarWatchContext solarWatchContext) : ISolarRepository
{
	private readonly SolarWatchContext _dbContext = solarWatchContext;

	public async Task<IEnumerable<Solar>> GetAll() =>
		await _dbContext.SolarInfo.ToListAsync();


	public async Task<Solar?> GetSolar(int cityId, DateTime date) =>
		await _dbContext.SolarInfo.FirstOrDefaultAsync(s => s.CityId == cityId && s.Date == date);



	public async Task<int> Add(Solar solar)
	{
		try
		{
			await _dbContext.AddAsync(solar);
			await _dbContext.SaveChangesAsync();

			return solar.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new InvalidDataException(e.Message);
		}
	}

	public async Task<bool> Delete(Solar solar)
	{
		try
		{
			_dbContext.Remove(solar);
			await _dbContext.SaveChangesAsync();
			return true;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new InvalidDataException(e.Message);
		}
	}

	public async Task<bool> Update(Solar solar)
	{
		try
		{
			_dbContext.Update(solar);
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