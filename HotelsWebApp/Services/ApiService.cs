using Blazored.LocalStorage;
using HotelsCommons.Models;
using HotelsWebApp.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HotelsWebApp.Services
{
    public class ApiService(HttpClient _httpClient, ILocalStorageService _localStorage)
    {
        public async Task<List<AllHotelsResult>> GetAllHotels()
        {
            var response = await _httpClient.GetAsync("api/Hotel/GetAllHotels");

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<List<AllHotelsResult>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }

            return new List<AllHotelsResult>();
        }

        public async Task<HotelDTO> GetHotel(string id)
        {
            var response = await _httpClient.GetAsync($"api/Hotel/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<HotelDTO>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }

            return null;
        }

        public async Task<HotelRoomResult> GetRoom(string id)
        {
            var response = await _httpClient.GetAsync($"api/Room/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<HotelRoomResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }

            return null;
        }

        private async Task<bool> IsAuthenticatedAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                return true;
            }
            return false;
        }
    }
}
