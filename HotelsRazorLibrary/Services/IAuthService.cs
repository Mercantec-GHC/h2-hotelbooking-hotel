using HotelsCommons.Models;

namespace HotelsRazorLibrary.Services
{
    public interface IAuthService
    {
        Task<HttpResponseMessage> Login(UserLoginDTO loginModel);
        Task Logout();
        Task<HttpResponseMessage> Register(UserCreateDTO registerModel);
    }
}
