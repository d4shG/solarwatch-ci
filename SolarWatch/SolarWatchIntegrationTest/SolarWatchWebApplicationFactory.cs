using System.Data.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SolarWatch.Models;
using SolarWatchIntegrationTest;

namespace SolarWatchTest.IntegrationTest;

public class SolarWatchWebApplicationFactory : WebApplicationFactory<Program>
{
	private readonly string _dbName = Guid.NewGuid().ToString();

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseEnvironment("Test");
		
		builder.ConfigureServices(services =>
		{
			var dbContextDescriptor = services.SingleOrDefault(
				d => d.ServiceType ==
				     typeof(IDbContextOptionsConfiguration<SolarWatchContext>));

			services.Remove(dbContextDescriptor);
			
			services.AddDbContext<SolarWatchContext>(options =>
			{
				options.UseInMemoryDatabase(_dbName);
			});
			
			using var scope = services.BuildServiceProvider().CreateScope();
			var solarContext = scope.ServiceProvider.GetRequiredService<SolarWatchContext>();
			solarContext.Database.EnsureDeleted();
			solarContext.Database.EnsureCreated();
			
			services.AddAuthentication("TestScheme")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });

			services.AddAuthorizationBuilder()
                .AddPolicy("TestPolicy", policy =>
					policy.RequireAuthenticatedUser());
			
		});
	}



}