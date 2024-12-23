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
    
    [Fact]
    public async Task RegisterUser_ShouldDisplayErrorForDuplicateTenant()
    {
        
        // Arrange
        var registrationDto = new UserRegistrationDto
        {
            Email = "testuser2@example.com",
            Password = "securepassword",
            Organisation = "TestOrganisation2",
            IsInvited = false
        };

        var jsonContent = JsonConvert.SerializeObject(registrationDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/User/register", content);
        
        response.EnsureSuccessStatusCode(); // Ensure we got a 2xx status code
        
        // Verify tenant was created
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == registrationDto.Organisation);
        Assert.NotNull(tenant); // Ensure tenant is not null
        
     
        // // Prepare data for new registration attempt
        // string newEmail = "newuser@example.com";
        // string newPassword = "Password123";
        // string duplicateOrganisation = preExistingOrganisation; // Same as the existing tenant
        //
        // // Act: Perform UI automation for registration
        _driver.Navigate().GoToUrl("http://localhost:3000/register"); // URL of your React app
        
        var emailInput = _driver.FindElement(By.Name("email"));
        var organisationInput = _driver.FindElement(By.Name("organisation"));
        var passwordInput = _driver.FindElement(By.Name("password"));
        var confirmPasswordInput = _driver.FindElement(By.Name("confirmPassword"));
        var submitButton = _driver.FindElement(By.CssSelector("button[type='submit']"));
        
        emailInput.SendKeys("testuser3@example.com");
        organisationInput.SendKeys("TestOrganisation2");
        passwordInput.SendKeys("Test101*&");
        confirmPasswordInput.SendKeys("Test101*&");
        
        submitButton.Click();
        
        // Wait for error message to appear
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var errorMessageElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("p.text-red-500"))); // Replace with your error message selector
        
        // Assert
        Assert.NotNull(errorMessageElement); // Ensure the error message is displayed
        Assert.Contains("An organisation with the name 'TestOrganisation2' already exists.", errorMessageElement.Text); // Verify error message content
        
        // Verify the database remains unchanged
        // var duplicateTenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Name == duplicateOrganisation);
        // Assert.NotNull(duplicateTenant); // The tenant should exist (pre-created one)
        // // Assert.Equal(preExistingTenant.Id, duplicateTenant.Id); // Ensure it's the same tenant
        //
        // var duplicateUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newEmail);
        // Assert.Null(duplicateUser); // No new user should be created
    }
    
}