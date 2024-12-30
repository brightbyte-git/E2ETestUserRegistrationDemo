using BackendAPI;
using BackendAPI.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EndToEndTests.Fixtures;

public class RegistrationTestsCollectionFixture : IDisposable
{
    public IWebDriver WebDriver { get; private set; }
    public E2EDemoDbContext DbContext { get; private set; }
    public UserRepository UserRepository { get; private set; }
    public HttpClient HttpClient { get; private set; }
    public DockerComposeFixture DockerComposeFixture { get; private set; }
    public DatabaseFixture DatabaseFixture { get; private set; }
    public CustomWebApplicationFactory<Program> Factory { get; private set; }

    public RegistrationTestsCollectionFixture()
    {
        // Initialize WebDriver
        WebDriver = new ChromeDriver();

        // Initialize DockerComposeFixture
        DockerComposeFixture = new DockerComposeFixture();

        // Build IConfiguration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        // Initialize DatabaseFixture with IConfiguration
        DatabaseFixture = new DatabaseFixture(configuration);

        // Access context and repositories from DatabaseFixture
        DbContext = DatabaseFixture.Context;
        UserRepository = DatabaseFixture.UserRepository;

        // Initialize CustomWebApplicationFactory
        Factory = new CustomWebApplicationFactory<Program>();

        // Initialize HttpClient
        HttpClient = Factory.CreateClient();
    }

    public void Dispose()
    {
        WebDriver.Quit();
        WebDriver.Dispose();
        DockerComposeFixture.StopDockerCompose();
        DatabaseFixture.Dispose();
        Factory.Dispose();
    }
}