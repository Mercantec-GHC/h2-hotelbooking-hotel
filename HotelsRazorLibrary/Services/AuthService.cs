using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using HotelsCommons.Models;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;

namespace HotelAdmin.WebView.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<HttpResponseMessage> Register(UserCreateDTO registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RegisterResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return response;
        }

        public async Task<UserResultDTO> GetUserInfo()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                Console.WriteLine(_httpClient.DefaultRequestHeaders.Authorization);

                var response = await _httpClient.GetAsync("api/Auth/me");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<UserResultDTO>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }
            return null;
        }

        public async Task<HttpResponseMessage> Login(UserLoginDTO loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await _httpClient.PostAsync("api/auth/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return response;
            }

            var loginResult = JsonSerializer.Deserialize<LoginResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            await _localStorage.SetItemAsync("refreshToken", loginResult.RefreshToken);
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
            
            return response;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        public async void CheckTokenExpiry()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var expiry = jsonToken?.ValidTo;

                Console.WriteLine($"Token expired: {expiry.HasValue && expiry.Value < DateTime.UtcNow}");

                if (expiry.HasValue && expiry.Value < DateTime.UtcNow)
                {
                    await Logout();
                }
            }
        }
    }
}
