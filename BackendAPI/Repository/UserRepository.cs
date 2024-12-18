using BackendAPI.Interface;
using BackendAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Repository;

public class UserRepository : IUserRepository
{
    private readonly E2EDemoDbContext _context;
    private readonly IConfiguration _configuration;

    public UserRepository(E2EDemoDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

   /// <summary>
/// Registers a new user to the database based on the UserRegistrationDto object.
/// </summary>
/// <param name="registrationDto">The UserRegistrationDto object containing the user's details.</param>
/// <returns>A Task representing the asynchronous operation, returning true if the user was successfully registered, false otherwise.</returns>
public async Task<bool> RegisterUserAsync(UserRegistrationDto registrationDto)
{
    if (registrationDto == null)
        throw new ArgumentNullException(nameof(registrationDto));

    // Check if the email is already registered
    var existingUser = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == registrationDto.Email);

    if (existingUser != null)
        throw new InvalidOperationException("Email is already in use.");

    Tenant tenant;

    if (!registrationDto.IsInvited)
    {
        // Check if the organization already exists
        tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Name == registrationDto.Organisation);

        if (tenant != null)
        {
            throw new InvalidOperationException($"An organisation with the name '{registrationDto.Organisation}' already exists.");
        }

        // Create a new tenant
        tenant = new Tenant { Name = registrationDto.Organisation };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
    }
    else
    {
        // For invited users, ensure the tenant exists
        tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Name == registrationDto.Organisation);

        if (tenant == null)
        {
            throw new InvalidOperationException("Invited user must belong to an existing tenant.");
        }
    }

    // Create the new user
    var user = new User
    {
        Email = registrationDto.Email,
        PasswordHash = HashPassword(registrationDto.Password),
        TenantId = tenant.Id,
        Organisation = registrationDto.Organisation,
        IsAdmin = !registrationDto.IsInvited,
        isInvited = registrationDto.IsInvited,
        CreatedAt = DateTime.UtcNow
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return true;
}
    
    ///     <summary>
    /// Hashes a plaintext password using the BCrypt algorithm and returns the hashed password.
    ///     </summary>
    ///     <param name="password">The plaintext <PASSWORD> to be hashed.</param>
    ///     <returns>A string representing the hashed password.</returns>
    public string HashPassword(string password)
    {
        // Implement your password hashing logic here
        return BCrypt.Net.BCrypt.HashPassword(password); // Example using BCrypt
    }
}