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

    public async Task<PagedResponse<UserProfileDto>> GetAllUsersAsync(int page, int pageSize)
    {
        var query = _repo.Query();
        int total = await query.CountAsync();
        var items = await query.OrderBy(u => u.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<UserProfileDto>(_mapper.Map<List<UserProfileDto>>(items), page, pageSize, total);
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
