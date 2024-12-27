using BackendAPI;
using BackendAPI.Repository;
using EndToEndTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EndToEndTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public E2EDemoDbContext Context { get; private set; }
    public UserRepository UserRepository { get; private set; }

    public DatabaseFixture(IConfiguration configuration)
    {
        // Retrieve the connection string from the configuration
        string connectionString = ConfigurationHelper.GetConnectionString("E2ETestsConnection");

        var options = new DbContextOptionsBuilder<E2EDemoDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        Context = new E2EDemoDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.Migrate();
        Context.Database.EnsureCreated();

        UserRepository = new UserRepository(Context, configuration); // Pass configuration if needed
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}