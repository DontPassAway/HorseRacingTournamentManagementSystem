using HorseRacing.Application.DTOs.RaceAssignments;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRaceAssignmentService
{
    Task<RaceAssignmentDto> AssignRefereeAsync(CreateRaceAssignmentDto dto);
    Task<PagedResponse<RaceAssignmentDto>> GetMyAssignmentsAsync(int refereeUserId, int page, int pageSize);
    Task DeleteAssignmentAsync(int id);
}
