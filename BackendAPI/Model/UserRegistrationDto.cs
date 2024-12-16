namespace BackendAPI.Model;

public class UserRegistrationDto
{
    public string Email { get; set; }
    public string Organisation { get; set; } // This will be used as the tenant name
    public string Password { get; set; }
    public bool IsInvited { get; set; }
}