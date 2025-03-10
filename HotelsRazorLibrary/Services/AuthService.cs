﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using HotelsCommons.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace HotelsRazorLibrary.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly bool _requireRoles;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _requireRoles = Config.RequireRoles;
        }

        public async Task<HttpResponseMessage> Register(UserCreateDTO registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);

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

            var loginResult = JsonSerializer.Deserialize<LoginResultDTO>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!hasAuthorization(loginResult.Token))
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("User is not authorized.")
                };
            }

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            await _localStorage.SetItemAsync("refreshToken", loginResult.RefreshToken);
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);
            
            return response;
        }

        public async Task<bool> UpdateUserInfo(string id, UserUpdateDTO userUpdateDTO)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                var response = await _httpClient.PatchAsJsonAsync($"api/auth/ChangeUserInfo?userId={id}", userUpdateDTO);
                return response.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task<bool> UpdateUserPassword(PasswordDTO passwordDTO)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                var response = await _httpClient.PatchAsJsonAsync($"api/auth/changePassword", passwordDTO);
                return response.IsSuccessStatusCode;
            }
            return false;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private bool hasAuthorization(string token)
        {
            var claims = CustomAuthenticationStateProvider.ParseClaimsFromJwt(token);
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            if (_requireRoles)
            {
                if (!roles.Any())
                {
                    return false;
                }
            }

            return true;
        }

        public async void CheckAccess()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                if (!hasAuthorization(token))
                {
                    await Logout();
                    return;
                }

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var expiry = jsonToken?.ValidTo;

                //Console.WriteLine($"Token expired: {expiry.HasValue && expiry.Value < DateTime.UtcNow}");

                if (expiry.HasValue && expiry.Value < DateTime.UtcNow)
                {
                    await Logout();
                }
            }
        }
    }
}
