using HorseRacing.Application.DTOs.RefereeReports;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRefereeReportService
{
    Task<RefereeReportDto> CreateReportAsync(int refereeUserId, CreateRefereeReportDto dto);
    Task<List<RefereeReportDto>> GetReportsByRaceAsync(int raceId);
    Task<RefereeReportDto> UpdateReportAsync(int id, int refereeUserId, UpdateRefereeReportDto dto);
    Task<PenaltyResultDto> ApplyPenaltyAsync(int reportId, int refereeUserId, CreatePenaltyDto dto);
}
