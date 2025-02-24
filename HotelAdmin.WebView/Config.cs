using Blazored.LocalStorage;
using HotelAdmin.WebView.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Config
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddBlazorBootstrap();

            //services.AddHttpClient();

            services.AddHotelLibrary();

            return services;
        }
    }
}
