using HorseRacing.Application.DTOs.RefereeProfiles;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RefereeProfileService : IRefereeProfileService
{
    private readonly IGenericRepository<User> _userRepo;

    public RefereeProfileService(IGenericRepository<User> userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<List<RefereeProfileDto>> GetAllRefereesAsync()
    {
        var referees = await _userRepo.Query()
            .Where(u => u.Role == UserRole.Referee && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return referees.Select(u => new RefereeProfileDto
        {
            UserId = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            IsActive = u.IsActive
        }).ToList();
    }
}
