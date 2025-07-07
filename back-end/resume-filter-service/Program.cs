
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using resume_filter_service.Services;
using resume_filter_service.Services.Interfaces;

namespace resume_filter_service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")  // Angular dev server
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Optional: if using cookies or auth
                });
            });
            builder.Services.AddControllers();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/v2.0";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuers =
                    [
                        $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0",
                        $"https://sts.windows.net/{builder.Configuration["AzureAd:TenantId"]}/"
                    ]
                };
                options.Audience = builder.Configuration["AzureAd:Audience"];
            });

            builder.Services.AddAuthorization();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Resume API", Version = "v1" });

                var azureAd = builder.Configuration.GetSection("AzureAd");
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{azureAd["Instance"]}{azureAd["TenantId"]}/oauth2/v2.0/authorize"),
                            TokenUrl = new Uri($"{azureAd["Instance"]}{azureAd["TenantId"]}/oauth2/v2.0/token"),
                            Scopes = new Dictionary<string, string>
                {
                    { $"api://{azureAd["ClientId"]}/resume.read", "Access Resume API" }
                }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        new[] { $"api://{azureAd["ClientId"]}/resume.read" }
                    }
                });
            });
            builder.Services.AddScoped<ITextExtractionService, TextExtractionService>();
            builder.Services.AddScoped<IScoringService, ScoringService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
                    c.OAuthUsePkce(); // Required for SPA-style auth
                });
            }
            
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularDev");
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
