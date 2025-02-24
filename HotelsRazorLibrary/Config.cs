namespace Microsoft.Extensions.DependencyInjection
{
    public static class Config
    {
        public static IServiceCollection AddHotelLibrary(this IServiceCollection services)
        {
            services.AddScoped<SidenavService>();

            return services;
        }
    }
}
