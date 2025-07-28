using IdentityService.Models;

namespace IdentityService.Services
{
    public interface IIdentityService
    {
        Task<LoginResponse> Authenticate(string username, string password);
    }
}