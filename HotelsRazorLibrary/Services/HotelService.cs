using HotelsCommons.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace HotelsRazorLibrary.Services
{
    public class HotelService
    {
        private readonly HttpClient _httpClient;

        public HotelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
                       
        }

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
    }
}
