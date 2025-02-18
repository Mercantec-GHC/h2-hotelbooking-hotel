

using System;


using System.Security.Cryptography.X509Certificates;
using System.Text;
using BackendAPI.Data;
using BackendAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace BackendAPI
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

            ConfigurationManager Configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? GetEnvOrSercret("DATABASE_CONNECTION_STRING");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                string jwtIssuer = Configuration["Jwt:Issuer"] ?? GetEnvOrSercret("JWT_ISSUER");
                string jwtKey = Configuration["Jwt:Key"] ?? GetEnvOrSercret("JWT_KEY");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "date" });
            //    c.MapType<DateTime?>(() => new OpenApiSchema { Type = "string", Format = "date" });
            //});

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                c.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "date" });
                c.MapType<DateTime?>(() => new OpenApiSchema { Type = "string", Format = "date" });

                // Tilføj JWT-autentificeringskonfiguration
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Indsæt 'Bearer {token}'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            //builder.Configuration.AddUserSecrets<Program>();
            var app = builder.Build();

            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                //app.ApplyMigrations();
            }

            app.InitializeDatabase(Configuration);

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

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
