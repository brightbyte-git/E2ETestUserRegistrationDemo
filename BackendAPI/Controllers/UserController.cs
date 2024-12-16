using BackendAPI.Interface;
using BackendAPI.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

[ApiController]
[Route("api/[controller]")]

public class UserController
{
    private readonly IUserRepository _userRepository;
    
    public UserController(E2EDemoDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    /// <summary>/// Registers a new user with the specified registration data and returns a success or failure response.
    /// </summary>
    /// <param name="registrationDto">The registration data containing user details such as email, password, and tenant information.</param>
    /// <returns>An IActionResult indicating whether the user was registered successfully or not with a message in the response body.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto registrationDto)
    {
        if (registrationDto == null)
            return BadRequest(new { Message = "Invalid registration data." });

        try
        {
            var success = await _userRepository.RegisterUserAsync(registrationDto);

            if (success)
                return Ok(new
                {
                    Message = registrationDto.IsInvited
                        ? "User registered successfully with existing tenant."
                        : "User registered successfully with new tenant."
                });
        }
        catch (InvalidOperationException ex)
        {
            // Handle known exceptions like email or tenant errors
            return Conflict(new { Message = ex.Message }); // 409 Conflict for already existing resources
        }
        catch (Exception ex)
        {
            // Handle unexpected excepœtions
            return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
        }

        return BadRequest(new { Message = "User registration failed." });
    }

}