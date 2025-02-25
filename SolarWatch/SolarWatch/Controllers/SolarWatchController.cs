using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarWatch.Services;

namespace SolarWatch.Controllers;

public class SolarWatchController(ILogger<SolarWatchController> logger, SolarWatchService solarWatchService) : ControllerBase
{
	private readonly ILogger<SolarWatchController> _logger = logger;
	private readonly SolarWatchService _solarWatchService = solarWatchService;


	[HttpGet("solar-watch", Name = "GetSolarWatchData"), Authorize]
	public async Task<IActionResult> GetSolarWatchData([Required]string city, [Required]DateTime date)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
    
		try
		{
			var isAdmin = User.IsInRole("Admin");
			var solarData = await _solarWatchService.GetSolarInfo(city, date, isAdmin);
			return Ok(solarData);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error occurred while fetching solar watch data.");
			return BadRequest(e.Message);
		}
	}
	
	[HttpGet("cities"), Authorize]
	public async Task<IActionResult> GetAllCities()
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}
    
		try
		{
			var cities = await _solarWatchService.GetAllCities();
			return Ok(cities);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Error occurred while fetching solar watch data.");
			return BadRequest(e.Message);
		}
	}
	
}