using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.Context.Repositories;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.Infrastructure.Data;
using StudentAccounting.WebApi.Services.Auth;
using StudentAccounting.WebApi.Services.Common;
using StudentAccounting.WebApi.Services.Contract;
using StudentAccounting.WebApi.Services.Education;
using StudentAccounting.WebApi.Services.Employee;
using StudentAccounting.WebApi.Services.Excel;
using StudentAccounting.WebApi.Services.Notifications;
using StudentAccounting.WebApi.Services.Organization;
using StudentAccounting.WebApi.Services.StudentTraining;
using StudentAccounting.WebApi.Services.TrainingGroup;
using StudentAccounting.WebApi.Services.TrainingProgram;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministratorOnly", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("MethodologistOnly", policy =>
        policy.RequireRole("Methodologist"));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<IStudentTrainingService, StudentTrainingService>();
builder.Services.AddScoped<ITrainingProgramService, TrainingProgramService>();
builder.Services.AddScoped<ITrainingGroupService, TrainingGroupService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<AuditService>();

//генерация и хранение договоров
builder.Services.AddSingleton(new StorageOptions
{
    ContractsRoot = Path.Combine(
        builder.Environment.ContentRootPath,
        builder.Configuration["StorageSettings:ContractsRoot"] ?? "storage/contracts")
});
builder.Services.AddSingleton(
    builder.Configuration.GetSection(ExecutorOptions.SectionName).Get<ExecutorOptions>() ?? new ExecutorOptions());
builder.Services.AddScoped<IContractDocumentGenerator, ContractDocumentGenerator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

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
