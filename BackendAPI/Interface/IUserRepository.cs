using BackendAPI.Model;

namespace BackendAPI.Interface;

public interface IUserRepository
{
    Task<bool> RegisterUserAsync(UserRegistrationDto registrationDto);

}