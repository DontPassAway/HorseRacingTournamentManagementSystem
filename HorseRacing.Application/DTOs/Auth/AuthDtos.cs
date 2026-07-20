using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Auth;

public record RegisterDto(
    string Username,
    string Email,
    string Password,
    string FullName,
    string? PhoneNumber,
    UserRole Role
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    int UserId,
    string Username,
    string Email,
    string FullName,
    UserRole Role,
    string Token
);

public class UserProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
