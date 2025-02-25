using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SolarWatch.Models;
using SolarWatch.Repositories;
using SolarWatch.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using SolarWatch.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AddServices(builder);
AddDbContext(builder);
AddAuthentication(builder);
AddIdentity(builder);
AddCookiePolicy(builder);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var authenticationSeeder = scope.ServiceProvider.GetRequiredService<AuthenticationSeeder>();
authenticationSeeder.AddRoles();
authenticationSeeder.AddAdmin();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void AddServices(WebApplicationBuilder webApplicationBuilder)
{
	webApplicationBuilder.Services.AddScoped<IAuthService, AuthService>();
	webApplicationBuilder.Services.AddScoped<ITokenService, TokenService>();
	webApplicationBuilder.Services.AddScoped<ISolarDataProvider, SunriseSunsetApi>();
	webApplicationBuilder.Services.AddScoped<ICityDataProvider, GeocodingApi>();
	webApplicationBuilder.Services.AddScoped<ISolarRepository, SolarRepository>();
	webApplicationBuilder.Services.AddScoped<ICityRepository, CityRepository>();
	webApplicationBuilder.Services.AddScoped<SolarWatchService>();
	webApplicationBuilder.Services.AddScoped<AuthenticationSeeder>();
}

void AddDbContext(WebApplicationBuilder builder1)
{
	builder1.Services.AddDbContext<SolarWatchContext>(options =>
	{
		options.UseSqlServer(
			"Server=localhost,1433;Database=SolarWatch;User Id=sa;Password=yourStrong(!)Password;Encrypt=false;",
			// Environment.GetEnvironmentVariable("DbConnectionString"),
			sqlOptions => sqlOptions.EnableRetryOnFailure(
				maxRetryCount: 5,
				maxRetryDelay: TimeSpan.FromSeconds(10),
				errorNumbersToAdd: null
			));
	});
}

void AddIdentity(WebApplicationBuilder webApplicationBuilder1)
{
	webApplicationBuilder1.Services
		.AddIdentityCore<IdentityUser>(options =>
		{
			options.SignIn.RequireConfirmedAccount = false;
			options.User.RequireUniqueEmail = true;
			options.Password.RequireDigit = false;
			options.Password.RequiredLength = 6;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequireLowercase = false;
		})
		.AddRoles<IdentityRole>()
		.AddEntityFrameworkStores<SolarWatchContext>();
}

void AddAuthentication(WebApplicationBuilder builder2)
{
	builder2.Services
		.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
		.AddJwtBearer(options =>
		{
			options.Events = new JwtBearerEvents
			{
				OnMessageReceived = context =>
				{
					var token = context.Request.Cookies["AuthToken"];
					if (!string.IsNullOrEmpty(token))
						context.Token = token;

					return Task.CompletedTask;
				}
			};

			options.TokenValidationParameters = new TokenValidationParameters()
			{
				ClockSkew = TimeSpan.Zero,
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = "apiWithAuthBackend",
				ValidAudience = "apiWithAuthBackend",
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes("!SomethingSecret!!SomethingSecret!")
				),
			};
		});
}

void AddCookiePolicy(WebApplicationBuilder builder3)
{
	builder3.Services.Configure<CookiePolicyOptions>(options =>
	{
		options.HttpOnly = HttpOnlyPolicy.Always;
		options.Secure = CookieSecurePolicy.Always;
		options.MinimumSameSitePolicy = SameSiteMode.None;
	});
}

public partial class Program
{
}