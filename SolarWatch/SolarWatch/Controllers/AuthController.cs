using Microsoft.AspNetCore.Mvc;
using SolarWatch.Contracts;
using SolarWatch.Services.Authentication;

namespace SolarWatch.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authenticationService) : ControllerBase
{
	[HttpPost("Register")]
	public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var result = await authenticationService.RegisterAsync(request.Email, request.Username, request.Password, request.Role);

		if (result.Success)
			return CreatedAtAction(nameof(Register), new RegistrationResponse(result.Email, result.UserName));
		
		AddErrors(result);
		return BadRequest(ModelState);

	}
	
	[HttpPost("Login")]
	public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
	{
		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var result = await authenticationService.LoginAsync(request.Email, request.Password);
		
		var cookieOptions = new CookieOptions
		{
			Expires = DateTime.UtcNow.AddHours(1)
		};

		Response.Cookies.Append("AuthToken", result.Token, cookieOptions);
		

		if (result.Success) 
			return Ok();
		
		AddErrors(result);
		return BadRequest(ModelState);
	}


	private void AddErrors(AuthResult result)
	{
		foreach (var error in result.ErrorMessages)
			ModelState.AddModelError(error.Key, error.Value);
	}
}
