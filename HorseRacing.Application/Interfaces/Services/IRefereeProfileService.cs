using HorseRacing.Application.DTOs.RefereeProfiles;

namespace HorseRacing.Application.Interfaces.Services;

public interface IRefereeProfileService
{
    Task<List<RefereeProfileDto>> GetAllRefereesAsync();
}
