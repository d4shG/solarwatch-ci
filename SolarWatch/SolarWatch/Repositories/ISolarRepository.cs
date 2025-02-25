using SolarWatch.Models;

namespace SolarWatch.Repositories;

public interface ISolarRepository
{
	Task<IEnumerable<Solar>> GetAll();
	Task<Solar?> GetSolar(int cityId, DateTime date);
	Task<int> Add(Solar solar);
	Task<bool> Delete(Solar solar);
	Task<bool> Update(Solar solar);
}