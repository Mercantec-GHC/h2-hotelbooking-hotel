using Blazored.LocalStorage;
using HotelAdmin.WebView.Models;
using HotelsCommons.Models;
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
