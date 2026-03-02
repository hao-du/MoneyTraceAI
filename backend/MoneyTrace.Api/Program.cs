using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MoneyTrace.Api.Endpoints;
using MoneyTrace.Application;
using MoneyTrace.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "MoneyTraceApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "MoneyTraceClient",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "super-secret-key-that-needs-to-be-at-least-256-bits-long-!"))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<MoneyTrace.Api.Middleware.ExceptionHandlingMiddleware>();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthenticationEndpoints();
app.MapBankEndpoints();
app.MapCounterpartyEndpoints();
app.MapCurrencyEndpoints();
app.MapSettingsEndpoints();
app.MapTransactionEndpoints();
app.MapDashboardEndpoints();

app.MapGet("/", () => "MoneyTrace API is running!");

app.Run();
