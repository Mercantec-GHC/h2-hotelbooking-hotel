using BackendAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using AppDbContext context = scope.ServiceProvider.GetService<AppDbContext>();

            context.Database.Migrate();
        }
    }
}
