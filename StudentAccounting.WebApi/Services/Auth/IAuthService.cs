using StudentAccounting.WebApi.DTOs.Auth;

namespace StudentAccounting.WebApi.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto> UpdateUserAsync(Guid id, UserUpdateDto dto);
    Task ToggleUserActiveAsync(Guid id);
    Task DeleteUserAsync(Guid id);
}