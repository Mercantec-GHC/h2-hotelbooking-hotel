using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

        //public async Task authToken()
        //{
        //    var token = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }
        //}

        public async Task<List<HotelDTO>> GetAllHotels()
        {

            try
            {
                var hotels = await _httpClient.GetFromJsonAsync<List<HotelDTO>>("/api/Hotel/GetAllHotels");
                return hotels ?? new List<HotelDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching hotels: {ex.Message}");
                return new List<HotelDTO>(); 
            }
        }
        public async Task<User> Login(UserLoginDTO request)
        {
            
                var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", request);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    return null;
                }         

            


            //if (_contextAccessor.HttpContext?.Response.HasStarted == false)
            //{
            //    _contextAccessor.HttpContext.Response.Cookies.Append("AuthToken", loginResponse.Token, new CookieOptions
            //    {
            //        HttpOnly = true,
            //        Secure = true,
            //        Expires = DateTime.UtcNow.AddHours(1)
            //    });
            //}
            //else
            //{
            //    Console.WriteLine("Response already started, cannot set cookie.");
            //}

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

            return loginResponse.User;
        }
    }
    
}       
   