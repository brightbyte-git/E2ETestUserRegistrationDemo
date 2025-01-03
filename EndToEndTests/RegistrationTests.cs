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

[Collection("Registration Tests Collection")]
public class RegistrationTests
{
    private readonly IWebDriver _driver;
    private readonly E2EDemoDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly HttpClient _client;
    private readonly DockerComposeFixture _dockerComposeFixture;
    private readonly DatabaseFixture _databaseFixture;
    private readonly CustomWebApplicationFactory<Program> _factory;
    
    public RegistrationTests(RegistrationTestsCollectionFixture fixture)
    {
        _driver = fixture.WebDriver;
        _context = fixture.DbContext;
        _userRepository = fixture.UserRepository;
        _client = fixture.HttpClient;
        _dockerComposeFixture = fixture.DockerComposeFixture;
        _databaseFixture = fixture.DatabaseFixture;
        _factory = fixture.Factory;
    }

    [Fact]
    public async Task RegisterUser_ShouldNavigateToLoginAfterSuccessfulRegistration()
    {
        // Arrange
        _driver.Navigate().GoToUrl("http://localhost:3000/register"); // URL of your React app
        
        // Get html elements of front end registration input boxes
        var emailInput = _driver.FindElement(By.Name("email"));
        var organisationInput = _driver.FindElement(By.Name("organisation"));
        var passwordInput = _driver.FindElement(By.Name("password"));
        var confirmPasswordInput = _driver.FindElement(By.Name("confirmPassword"));
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

        string email = "testuser@example.com";
        string password = "Password123";
        string organisation = "Test Organisation";
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
    
    [Fact]
    public async Task RegisterUser_ShouldDisplayErrorForDuplicateTenant()
    {
        // Arrange
        var registrationDto = new UserRegistrationDto
        {
            Email = "testuser2@example.com",
            Password = "securepassword",
            Organisation = "Test Organisation",
            IsInvited = false
        };
        
        // TODO: Also write an assertion check to the Users table to ensure the registered user has been added

        // Verify tenant was created
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == registrationDto.Organisation);
        Assert.NotNull(tenant); // Ensure tenant is not null

        // Act: Attempt to register a duplicate user with the same organisation
        _driver.Navigate().GoToUrl("http://localhost:3000/register"); // URL of your React app

        var emailInput = _driver.FindElement(By.Name("email"));
        var organisationInput = _driver.FindElement(By.Name("organisation"));
        var passwordInput = _driver.FindElement(By.Name("password"));
        var confirmPasswordInput = _driver.FindElement(By.Name("confirmPassword"));
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));

        emailInput.SendKeys("testuser3@example.com");
        organisationInput.SendKeys("Test Organisation");
        passwordInput.SendKeys("Test101*&");
        confirmPasswordInput.SendKeys("Test101*&");

        submitButton.Click();

        // Wait for error message to appear
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var errorMessageElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("p.text-red-500"))); // Replace with your error message selector

        // Assert: Verify error message is displayed
        Assert.NotNull(errorMessageElement); // Ensure the error message is displayed
        Assert.Contains("An organisation with the name 'Test Organisation' already exists.", errorMessageElement.Text); // Verify error message content

        // Assert: Verify duplicate tenant was not created
        var duplicateTenant = await _context.Tenants.CountAsync(t => t.Name == "Test Organisation");
        Assert.Equal(1, duplicateTenant); // Ensure there is only one tenant with the name

        // Assert: Verify user details were not added
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "testuser3@example.com");
        Assert.Null(user); // Ensure user does not exist in the database
    }
    
}