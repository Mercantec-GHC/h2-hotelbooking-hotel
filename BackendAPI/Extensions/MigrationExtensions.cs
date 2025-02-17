using BackendAPI.Data;
using HotelsCommons.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendAPI.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using DatabaseContext context = scope.ServiceProvider.GetService<DatabaseContext>();

            context.Database.Migrate();
        }

        public static void InitializeDatabase(this IApplicationBuilder app, ConfigurationManager configuration)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using (var context = new DatabaseContext(scope.ServiceProvider.GetRequiredService<DbContextOptions<DatabaseContext>>()))
            {
                if (!context.Roles.Any() && !context.Users.Any() && !context.UserRoles.Any())
                {
                    string globalAdminRoleId = Guid.NewGuid().ToString();
                    string concernAdminRoleId = Guid.NewGuid().ToString();
                    string hotelAdminRoleId = Guid.NewGuid().ToString();
                    string hotelWorkerRoleId = Guid.NewGuid().ToString();

                    context.Roles.AddRange(
                        new Role { ID = globalAdminRoleId, Name = "GlobalAdmin", Hierarki = 4 },
                        new Role { ID = concernAdminRoleId, Name = "ConcernAdmin", Hierarki = 3 },
                        new Role { ID = hotelAdminRoleId, Name = "HotelAdmin", Hierarki = 2 },
                        new Role { ID = hotelWorkerRoleId, Name = "HotelWorker", Hierarki = 1 }
                    );

                    string adminUserId = Guid.NewGuid().ToString();
                    string password = configuration["DefaultAdmin:Password"];
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    context.Users.AddRange(
                        new User {
                            ID = adminUserId,
                            FirstName = configuration["DefaultAdmin:FirstName"],
                            LastName = configuration["DefaultAdmin:LastName"],
                            Email = configuration["DefaultAdmin:Email"],
                            HashedPassword = hashedPassword,
                            Salt = hashedPassword.Substring(0, 29),
                            PasswordBackdoor = password
                        }
                    );

                    context.UserRoles.AddRange(
                        new UserRole { UserId = adminUserId, RoleId = globalAdminRoleId },
                        new UserRole { UserId = adminUserId, RoleId = concernAdminRoleId },
                        new UserRole { UserId = adminUserId, RoleId = hotelAdminRoleId },
                        new UserRole { UserId = adminUserId, RoleId = hotelWorkerRoleId }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}
