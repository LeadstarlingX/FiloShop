using FiloShop.SharedKernel.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiloShop.Infrastructure.Persistence.AppDbContext;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=filoshop_docker;User Id=postgres;Password=postgres");
        return new ApplicationDbContext(optionsBuilder.Options, new SystemDateTimeProvider());
    }
}