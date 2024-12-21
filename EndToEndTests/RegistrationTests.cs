using System.Text;
using BackendAPI;
using BackendAPI.Model;
using BackendAPI.Repository;
using EndToEndTests.Fixtures;
using EndToEndTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace EndToEndTests;

public class RegistrationTests : IClassFixture<DockerComposeFixture>, IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    private readonly IWebDriver _driver;
    private readonly E2EDemoDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly HttpClient _client;
    private readonly DockerComposeFixture _dockerComposeFixture;
    private readonly DatabaseFixture _databaseFixture;
    private readonly CustomWebApplicationFactory<Program> _factory;
    
    public RegistrationTests(DockerComposeFixture dockerComposeFixture,  CustomWebApplicationFactory<Program> factory)
    {
        // Initialize the Chrome WebDriver
        _driver = new ChromeDriver();

        _dockerComposeFixture = dockerComposeFixture;

        // Use the helper to get the connection string
        string connectionString = ConfigurationHelper.GetConnectionString("E2ETestsConnection");

        // Pass the connection string to the DatabaseFixture
        _databaseFixture = new DatabaseFixture(connectionString);
        // _factory = new CustomWebApplicationFactory<Program>(connectionString);

        // Access the context and repositories from the DatabaseFixture
        _context = _databaseFixture.Context;
        _userRepository = _databaseFixture.UserRepository;
        _client = factory.CreateClient();
        // _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_ShouldNavigateToLoginAfterSuccessfulRegistration()
    {
        
        // Arrange
        _driver.Navigate().GoToUrl("http://localhost:3000/register"); // URL of your React app
        
        var emailInput = _driver.FindElement(By.Name("email"));
        var organisationInput = _driver.FindElement(By.Name("organisation"));
        var passwordInput = _driver.FindElement(By.Name("password"));
        var confirmPasswordInput = _driver.FindElement(By.Name("confirmPassword"));
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

        string email = "testuser@example.com";
        string password = "Password123";
        string organisation = "Test Organisation 2";
        bool isInvited = false;

        var registrationDto = new UserRegistrationDto
        {
            Email = email,
            Password = password,
            Organisation = organisation,
            IsInvited = isInvited
        };

        // Act
        emailInput.SendKeys(email);
        organisationInput.SendKeys(organisation);
        passwordInput.SendKeys(password);
        confirmPasswordInput.SendKeys(password);

        submitButton.Click();

        // Wait for the success message to appear
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("p.text-green-500")));

        // Wait for navigation to the login page (check for a URL change or an element specific to the login page)
        wait.Until(ExpectedConditions.UrlToBe("http://localhost:3000/login"));

        // Assert
        Assert.Equal("http://localhost:3000/login", _driver.Url); // Ensure navigation occurred

        // Verify tenant was created
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == registrationDto.Organisation);
        Assert.NotNull(tenant); // Ensure tenant is not null

        // Verify user was created and linked to the tenant
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registrationDto.Email);
        Assert.NotNull(user); // Ensure user is not null
        Assert.Equal(registrationDto.Email, user.Email);
        Assert.Equal(tenant.Id, user.TenantId); // Check if TenantId matches
    }
    
    public void Dispose()
    {
        // Cleanup after tests
        _driver.Quit();
        _dockerComposeFixture.StopDockerCompose();
        _databaseFixture.Dispose();
    }
}