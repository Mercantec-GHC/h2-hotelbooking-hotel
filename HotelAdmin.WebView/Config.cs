using HotelAdmin.WebView.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Config
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddBlazorBootstrap();

            services.AddHotelLibrary(requireRoles:true);

            services.AddScoped<ApiService>();
            services.AddHttpClient<ApiService>(client =>
            {
                client.BaseAddress = new Uri("https://10.135.71.51:5101");
                //client.BaseAddress = new Uri("https://localhost:7090");
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
