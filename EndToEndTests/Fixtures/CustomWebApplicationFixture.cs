using BackendAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EndToEndTests.Fixtures;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        // Set up the configuration
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.EndToEndTest.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
        
        builder.ConfigureServices((context, services) =>
        {
            var connectionString = context.Configuration.GetConnectionString("E2ETestsConnection");
            
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
                options.UseSqlServer(connectionString);
            });
        });
    }
}