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

public record UserProfileDto(
    int Id,
    string Username,
    string Email,
    string FullName,
    string? PhoneNumber,
    string? AvatarUrl,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt
);
