using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IUserService
{
    Task<PagedResponse<UserProfileDto>> GetAllUsersAsync(int page, int pageSize);
    Task<UserProfileDto> GetUserByIdAsync(int id);
    Task<UserProfileDto> UpdateUserRoleAsync(int userId, UserRole newRole);
    Task<UserProfileDto> ToggleUserActiveAsync(int userId);
    Task DeleteUserAsync(int userId);
}
