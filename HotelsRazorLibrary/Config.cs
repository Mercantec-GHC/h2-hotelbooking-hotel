using Blazored.LocalStorage;
using HotelsRazorLibrary.Services;
//using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Config
    {
        public static bool RequireRoles;

        public static IServiceCollection AddHotelLibrary(this IServiceCollection services, bool requireRoles = false)
        {
            RequireRoles = requireRoles;

            services.AddBlazoredLocalStorage();

            services.AddScoped<SidenavService>();

            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://10.135.71.51:5101");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            });

            return services;
        }
    }
}
