namespace SolarWatch.Models;

public class Solar
{
	public int Id { get; init; }
	public int CityId { get; init; }
	public string Sunrise { get; init; }
	public string Sunset { get; init; }
	public DateTime Date { get; init; }
}