using Blazored.LocalStorage;
using HotelsCommons.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelAdmin.WebView.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ApiService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<List<UserResultDTO>> GetAllUsers()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync("api/Auth/ListOtherUsers");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<UserResultDTO>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return new List<UserResultDTO>();
        }

        public async Task<List<HotelDTO>> GetAllHotels()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync("api/Hotel/GetAllHotels");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<HotelDTO>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return new List<HotelDTO>();
        }

        public async Task<bool> CreateHotel(CreateHotelDTO hotelDTO)
        {
            if (await IsAuthenticatedAsync())
            {

                var json = JsonSerializer.Serialize(hotelDTO);
                var response = await _httpClient.PostAsync("api/hotel/createhotel", new StringContent(json, Encoding.UTF8, "application/json"));

                return response.IsSuccessStatusCode;
            }

            return false;
        }

        public async Task<List<RoomResult>> GetRooms(string id)
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Hotel/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<HotelDTO>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result.Rooms;
                }
            }

            return new List<RoomResult>();
        }

        public async Task<RoomResult> GetRoom(string id)
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Room/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<RoomResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return null;
        }

        public async Task<List<BookingResult>> GetAllBookings()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Booking/GetAllBookings");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<BookingResult>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return new List<BookingResult>();
        }

        public async Task<BookingResult> GetBooking(string id)
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Booking/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<BookingResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return null;
        }

        public async Task<List<AllTicketsResult>> GetAllTickets()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync("api/Ticket/listTickets");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<AllTicketsResult>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return new List<AllTicketsResult>();
        }

        public async Task<TicketResult> GetTicket(string id)
        {
            if (id != null && await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Ticket/getTicket?ticketId={id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<TicketResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return null;
        }

        public async Task<HttpResponseMessage> UpdateStatus(string id, string status)
        {
            await IsAuthenticatedAsync();
            TicketStatusDTO statusModel = new TicketStatusDTO { Status = status };
            var json = JsonSerializer.Serialize(statusModel);
            var response = await _httpClient.PatchAsync($"api/ticket/updatestatus?ticketid={id}", new StringContent(json, Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<HttpResponseMessage> CreateMessage(string id, string message)
        {
            await IsAuthenticatedAsync();
            MessageCreateDTO statusModel = new MessageCreateDTO
            {
                TicketId = id,
                Message = message
            };
            var json = JsonSerializer.Serialize(statusModel);
            var response = await _httpClient.PostAsync("api/ticket/createmessage", new StringContent(json, Encoding.UTF8, "application/json"));

            return response;
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
