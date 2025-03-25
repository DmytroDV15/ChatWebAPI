
using ChatWebAPI.Models;

namespace ChatWebAPI.Services
{
    public interface IAuthService
    {
        Task RegisterUserAsync(string name, string email, string password);
        Task<RegistrationModel> AuthenticateUserAsync(string email, string password);
    }

}
