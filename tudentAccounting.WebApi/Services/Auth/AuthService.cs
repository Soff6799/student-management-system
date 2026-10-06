using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentAccounting.WebApi.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == dto.Login);
        if (user == null || !user.IsActive) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;
        user.LastLoginDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var token = GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Login = user.Login,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Role = user.Role.ToString()
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Login == dto.Login))
            throw new Exception("Пользователь с таким логином уже существует");

        var user = new User
        {
            Login = dto.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName,
            Role = dto.Role,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Login = user.Login,
            FullName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim(),
            Role = user.Role.ToString()
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiresMinutes = jwtSettings["ExpiresInMinutes"];

        Console.WriteLine("=== GenerateJwtToken ===");
        Console.WriteLine($"SecretKey: '{secretKey}' (длина: {secretKey.Length})");
        Console.WriteLine($"Issuer: '{issuer}'");
        Console.WriteLine($"Audience: '{audience}'");
        Console.WriteLine($"ExpiresInMinutes: '{expiresMinutes}'");
        Console.WriteLine("========================");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(double.Parse(expiresMinutes!)),
        signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}