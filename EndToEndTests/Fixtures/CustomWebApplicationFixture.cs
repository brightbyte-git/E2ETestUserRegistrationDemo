using BackendAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EndToEndTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Remove the existing DbContext configuration
            var serviceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<E2EDemoDbContext>));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            // Use the provided connection string for the test database
            services.AddDbContext<E2EDemoDbContext>(options =>
            {
                options.UseSqlServer(_connectionString);
            });
        });
    }
}