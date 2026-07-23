using AutoMapper;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IGenericRepository<User> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResponse<UserProfileDto>> GetAllUsersAsync(int page, int pageSize, string? search, bool? isActive)
    {
        var query = _repo.Query();

        // Xử lý logic search theo email, tên hoặc username
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(u =>
                (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                (u.FullName != null && u.FullName.ToLower().Contains(searchLower)) ||
                (u.Username != null && u.Username.ToLower().Contains(searchLower))
            );
        }

        // Xử lý filter trạng thái
        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        int total = await query.CountAsync();
        var items = await query.OrderBy(u => u.Id)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new PagedResponse<UserProfileDto>(_mapper.Map<List<UserProfileDto>>(items), page, pageSize, total);
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        // Nhóm theo Role từ DB để tối ưu số lần quét dữ liệu, tránh tải toàn bộ table lên RAM
        var roleGroups = await _repo.Query()
            .GroupBy(u => u.Role)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .ToListAsync();

        int GetCount(UserRole role) => roleGroups.FirstOrDefault(r => r.Role == role)?.Count ?? 0;

        var stats = new UserStatsDto
        {
            Total = roleGroups.Sum(r => r.Count),
            Admin = GetCount(UserRole.Admin),
            Owner = GetCount(UserRole.HorseOwner),
            Jockey = GetCount(UserRole.Jockey),
            Referee = GetCount(UserRole.Referee),
            Spectator = GetCount(UserRole.Spectator)
        };

        return stats;
    }

    public async Task<UserProfileDto> GetUserByIdAsync(int id)
    {
        var user = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(User), id);
        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserProfileDto> UpdateUserRoleAsync(int userId, UserRole newRole)
    {
        var user = await _repo.GetByIdAsync(userId) ?? throw new NotFoundException(nameof(User), userId);
        user.Role = newRole; user.UpdatedAt = DateTime.UtcNow;
        _repo.Update(user); await _uow.SaveChangesAsync();
        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserProfileDto> ToggleUserActiveAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId) ?? throw new NotFoundException(nameof(User), userId);
        user.IsActive = !user.IsActive; user.UpdatedAt = DateTime.UtcNow;
        _repo.Update(user); await _uow.SaveChangesAsync();
        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId) ?? throw new NotFoundException(nameof(User), userId);
        _repo.Remove(user); await _uow.SaveChangesAsync();
    }
}