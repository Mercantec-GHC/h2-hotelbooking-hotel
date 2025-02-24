using HotelsWebApp.AuthServices;
using HotelsWebApp.Components;
using Microsoft.Extensions.Hosting;
using System;
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

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            //builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            //builder.Services.AddSingleton<AuthService>();
            builder.Services.AddHttpClient<AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://10.135.71.51:5101"); 
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                Console.WriteLine("Test");
                 return new HttpClientHandler
                 {
                  
                     ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                 };
             });

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

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
