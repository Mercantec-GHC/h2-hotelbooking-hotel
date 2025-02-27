using Blazored.LocalStorage;
using HotelAdmin.WebView.Models;
using HotelsCommons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
