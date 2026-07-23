using AutoMapper;
using HorseRacing.Application.DTOs.RefereeReports;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

public class RefereeReportService : IRefereeReportService
{
    private readonly IGenericRepository<RefereeReport> _repo;
    private readonly IGenericRepository<Registration> _regRepo;
    private readonly IGenericRepository<RaceResult> _resultRepo;
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public RefereeReportService(IGenericRepository<RefereeReport> repo,
        IGenericRepository<Registration> regRepo,
        IGenericRepository<RaceResult> resultRepo,
        IUnitOfWork uow, IMapper mapper)
    {
        _repo = repo; _regRepo = regRepo; _resultRepo = resultRepo; _uow = uow; _mapper = mapper;
    }

    private IQueryable<RefereeReport> BaseQuery() => _repo.Query()
        .Include(r => r.Race).Include(r => r.RefereeUser);

    public async Task<RefereeReportDto> CreateReportAsync(int refereeUserId, CreateRefereeReportDto dto)
    {
        var report = new RefereeReport
        {
            RaceId = dto.RaceId, RefereeUserId = refereeUserId,
            Content = dto.Content, HasViolation = dto.HasViolation,
            ViolationType = dto.ViolationType, ViolationDescription = dto.ViolationDescription,
            ViolatingRegistrationId = dto.ViolatingRegistrationId
        };
        await _repo.AddAsync(report); await _uow.SaveChangesAsync();
        var created = await BaseQuery().FirstOrDefaultAsync(r => r.Id == report.Id);
        return _mapper.Map<RefereeReportDto>(created!);
    }

    public async Task<List<RefereeReportDto>> GetReportsByRaceAsync(int raceId)
    {
        var reports = await BaseQuery().Where(r => r.RaceId == raceId).ToListAsync();
        return _mapper.Map<List<RefereeReportDto>>(reports);
    }

    public async Task<RefereeReportDto> UpdateReportAsync(int id, int refereeUserId, UpdateRefereeReportDto dto)
    {
        var report = await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(RefereeReport), id);
        if (report.RefereeUserId != refereeUserId) throw new ForbiddenException();
        report.Content = dto.Content; report.HasViolation = dto.HasViolation;
        report.ViolationType = dto.ViolationType; report.ViolationDescription = dto.ViolationDescription;
        report.ViolatingRegistrationId = dto.ViolatingRegistrationId;
        report.IsFinalized = dto.IsFinalized; report.UpdatedAt = DateTime.UtcNow;
        _repo.Update(report); await _uow.SaveChangesAsync();
        var updated = await BaseQuery().FirstOrDefaultAsync(r => r.Id == id);
        return _mapper.Map<RefereeReportDto>(updated!);
    }

    public async Task<PenaltyResultDto> ApplyPenaltyAsync(int reportId, int refereeUserId, CreatePenaltyDto dto)
    {
        var report = await _repo.GetByIdAsync(reportId)
            ?? throw new NotFoundException(nameof(RefereeReport), reportId);

        if (report.RefereeUserId != refereeUserId)
            throw new ForbiddenException("Only the assigned referee can apply penalties.");

        var registration = await _regRepo.Query()
            .Include(r => r.Horse)
            .Include(r => r.Jockey).ThenInclude(j => j!.User)
            .FirstOrDefaultAsync(r => r.Id == dto.RegistrationId)
            ?? throw new NotFoundException(nameof(Registration), dto.RegistrationId);

        bool disqualified = dto.PenaltyType.Equals("Disqualified", StringComparison.OrdinalIgnoreCase);

        if (disqualified)
        {
            var result = await _resultRepo.FirstOrDefaultAsync(rr => rr.RegistrationId == dto.RegistrationId);
            if (result != null)
            {
                result.Disqualified = true;
                result.DisqualificationReason = dto.Reason;
                result.UpdatedAt = DateTime.UtcNow;
                _resultRepo.Update(result);
            }
        }

        await _uow.SaveChangesAsync();

        return new PenaltyResultDto
        {
            ReportId = reportId,
            RegistrationId = dto.RegistrationId,
            HorseName = registration.Horse.Name,
            JockeyName = registration.Jockey?.User?.FullName,
            PenaltyType = dto.PenaltyType,
            Reason = dto.Reason,
            HorseDisqualified = disqualified,
            AppliedAt = DateTime.UtcNow
        };
    }
}
