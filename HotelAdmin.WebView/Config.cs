﻿namespace Microsoft.Extensions.DependencyInjection
{
    public static class Config
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services)
        {
            services.AddBlazorBootstrap();

            services.AddHotelLibrary(requireRoles:true);

            return services;
        }
    }
}
