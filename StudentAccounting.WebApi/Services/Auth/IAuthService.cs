using StudentAccounting.WebApi.DTOs.Auth;

namespace StudentAccounting.WebApi.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
}