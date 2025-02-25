
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SolarWatch.Models;

public class SolarWatchContext(DbContextOptions<SolarWatchContext> options) : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
	public DbSet<City> Cities { get; set; }
	public DbSet<Solar> SolarInfo { get; set; }
}