using AutoMapper;
using HorseRacing.Application.DTOs.Profiles;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using HorseRacing.Shared.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class JockeyProfileService : IJockeyProfileService
{
    private readonly IGenericRepository<JockeyProfile> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public JockeyProfileService(IGenericRepository<JockeyProfile> repo, IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _uow = uow; _mapper = mapper;
    }

    public async Task<PagedResponse<JockeyProfileDto>> GetAllJockeysAsync(int page, int pageSize)
    {
        var query = _repo.Query().Include(j => j.User);
        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedResponse<JockeyProfileDto>(_mapper.Map<List<JockeyProfileDto>>(items), page, pageSize, total);
    }

    public async Task<JockeyProfileDto> GetJockeyByIdAsync(int id)
    {
        var profile = await _repo.Query().Include(j => j.User).FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new NotFoundException(nameof(JockeyProfile), id);
        return _mapper.Map<JockeyProfileDto>(profile);
    }

    public async Task<JockeyProfileDto> GetMyJockeyProfileAsync(int userId)
    {
        var profile = await _repo.Query().Include(j => j.User).FirstOrDefaultAsync(j => j.UserId == userId)
            ?? throw new NotFoundException(nameof(JockeyProfile), userId); 
        return _mapper.Map<JockeyProfileDto>(profile);
    }

    public async Task<JockeyProfileDto> UpdateMyJockeyProfileAsync(int userId, UpdateJockeyProfileDto dto)
    {
        var profile = await _repo.Query().Include(j => j.User).FirstOrDefaultAsync(j => j.UserId == userId)
            ?? throw new NotFoundException(nameof(JockeyProfile), userId); 

        profile.ExperienceYears = dto.ExperienceYears;
        profile.Weight = dto.Weight;
        profile.LicenseNumber = dto.LicenseNumber;
        profile.Nationality = dto.Nationality;

        _repo.Update(profile);
        await _uow.SaveChangesAsync();
        return _mapper.Map<JockeyProfileDto>(profile);
    }
}
