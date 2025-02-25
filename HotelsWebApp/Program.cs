
using HotelsWebApp.Auth;
using HotelsWebApp.Components;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace HotelsWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? certPasswordFile = Environment.GetEnvironmentVariable("CERT_PASSWORD");
            
            if (!string.IsNullOrEmpty(certPasswordFile) && File.Exists(certPasswordFile))
            {
                string certPassword = File.ReadAllText(certPasswordFile).Trim();

                builder.WebHost.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        string? certPath = Environment.GetEnvironmentVariable("CERT_PATH");
                        httpsOptions.ServerCertificate = new X509Certificate2(certPath, certPassword);
                    });
                });
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "jwt";
                options.DefaultChallengeScheme = "jwt";
            }).AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("jwt", options => { });

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHttpContextAccessor();           
            builder.Services.AddHotelLibrary();
           
            var app = builder.Build();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
