using Microsoft.EntityFrameworkCore;
using RealRender.ProductApiService.Models;
namespace RealRender.ProductApiService.Db;

public class ApplicationDbContext : DbContext
{
	public DbSet<Product> Products { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context)
		: base(context)
	{

	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}
}
