using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;
namespace HotelsWebApp.AuthServices
 
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
      


        public AuthService(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task authToken()
        {
            var token = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<User> Login(UserLoginDTO request)
        {
            var response = await _httpClient.PostAsJsonAsync("https://10.135.71.51:5101/api/Auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var token = await response.Content.ReadAsStringAsync();

            
            _contextAccessor.HttpContext?.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return await response.Content.ReadFromJsonAsync<User>();
        }
    }
    
}       
   