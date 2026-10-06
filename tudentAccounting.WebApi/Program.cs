using Microsoft.AspNetCore.Authentication.JwtBearer;
using StudentAccounting.Context.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.Services.Auth;
using System.Text;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.WebApi.Services.Organization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("SecretKey не найден");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

Console.WriteLine($"=== JWT Settings ===");
Console.WriteLine($"SecretKey length: {secretKey.Length}");
Console.WriteLine($"Issuer: '{issuer}'");
Console.WriteLine($"Audience: '{audience}'");
Console.WriteLine($"====================");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("=== Authentication Failed ===");
            Console.WriteLine($"Exception: {context.Exception.GetType().Name}");
            Console.WriteLine($"Message: {context.Exception.Message}");
            Console.WriteLine($"Token: {context.Request.Headers["Authorization"]}");
            Console.WriteLine("=============================");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("=== Token Validated Successfully ===");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine("=== OnChallenge (401) ===");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"ErrorDescription: {context.ErrorDescription}");
            Console.WriteLine($"Authorization Header: '{context.Request.Headers["Authorization"]}'");
            Console.WriteLine("=== All Request Headers ===");
            foreach (var header in context.Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }
            Console.WriteLine("=========================");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "v1";
    config.Title = "StudentAccounting.WebApi";
    config.Version = "v1";

    config.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Authorization");
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IOrganizationService, OrganizationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
