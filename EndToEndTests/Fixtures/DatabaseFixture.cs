using BackendAPI;
using BackendAPI.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EndToEndTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public E2EDemoDbContext Context { get; private set; }
    public UserRepository UserRepository { get; private set; }

    public DatabaseFixture(string connectionString)
    {
        var builder = WebApplication.CreateBuilder(new string[] { });

        // Clear default configuration sources
        builder.Configuration.Sources.Clear();

        builder.Configuration
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.EndToEndTest.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var options = new DbContextOptionsBuilder<E2EDemoDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        Context = new E2EDemoDbContext(options);
        Context.Database.EnsureDeleted();
        Context.Database.Migrate();
        Context.Database.EnsureCreated();

        UserRepository = new UserRepository(Context, builder.Configuration);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}