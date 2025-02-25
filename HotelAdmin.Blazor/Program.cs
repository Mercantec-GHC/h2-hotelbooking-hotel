using HotelAdmin.Blazor.Components;
using HotelAdmin.WebView.Components;
using HotelsRazorLibrary.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HotelAdmin.Blazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Services.AddAuthentication();

            ConfigurationManager Configuration = builder.Configuration;

            builder.Services.AddSharedServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddAdditionalAssemblies(typeof(Routes).Assembly)
                .AddInteractiveServerRenderMode();

            app.Run();
        }

        public static string? GetEnvOrSercret(string secret)
        {
            string? secretPath = Environment.GetEnvironmentVariable(secret);
            if (!string.IsNullOrEmpty(secretPath) && File.Exists(secretPath))
            {
                return File.ReadAllText(secretPath).Trim();
            }
            return null;
        }
    }
}
