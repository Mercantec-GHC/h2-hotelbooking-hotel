﻿using Blazored.LocalStorage;
using HotelsCommons.Models;
using HotelWeb.Blazor.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HotelWeb.Blazor.Services
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

        public async Task<RoomResult> GetRoom(string id)
        {
            var response = await _httpClient.GetAsync($"api/Room/{id}");

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<RoomResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }

            return null;
        }
        public async Task<BookingResult> GetBooking(string id)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

                var response = await _httpClient.GetAsync($"api/Booking/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<BookingResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }
            return null;
        }

        public async Task<CreateBookingResult> CreateBooking(CreateBookingDTO bookingModel)
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(savedToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

                var response = await _httpClient.PostAsJsonAsync("api/booking/createbooking", bookingModel);

                var result = JsonSerializer.Deserialize<CreateBookingResult>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result;
            }
            return null;
        }

        public async Task<List<BookingResult>> GetMyBookings()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync($"api/Booking/GetMyBookings");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<BookingResult>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return new List<BookingResult>();
        }

        public async Task<List<AllTicketsResult>> GetAllTickets()
        {
            if (await IsAuthenticatedAsync())
            {
                var response = await _httpClient.GetAsync("api/Ticket/MyTickets");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<List<AllTicketsResult>>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result;
                }
            }

            return null;
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

        public async Task<HttpResponseMessage> CreateMessage(string id, string message)
        {
            await IsAuthenticatedAsync();

            Console.WriteLine(_httpClient.DefaultRequestHeaders.Authorization);

            MessageCreateDTO messageModel = new MessageCreateDTO
            {
                TicketId = id,
                Message = message
            };
            var json = JsonSerializer.Serialize(messageModel);
            var response = await _httpClient.PostAsync("api/ticket/createmessage", new StringContent(json, Encoding.UTF8, "application/json"));

            return response;
        }

        public async Task<bool> CreateTicket(TicketCreateDTO ticketModel)
        {
            if (await IsAuthenticatedAsync())
            {
                var json = JsonSerializer.Serialize(ticketModel);
                var response = await _httpClient.PostAsync("api/ticket/createticket", new StringContent(json, Encoding.UTF8, "application/json"));

                return response.IsSuccessStatusCode;
            }

            return false;
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
