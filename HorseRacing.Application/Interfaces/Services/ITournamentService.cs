using HorseRacing.Application.DTOs.Races;
using HorseRacing.Application.DTOs.Tournaments;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;

namespace HorseRacing.Application.Interfaces.Services;

public interface ITournamentService
{
    Task<TournamentDto> CreateTournamentAsync(CreateTournamentDto dto);
    Task<TournamentDto> GetTournamentByIdAsync(int id);
    Task<PagedResponse<TournamentDto>> GetAllTournamentsAsync(int page, int pageSize);
    Task<TournamentDto> UpdateTournamentAsync(int id, UpdateTournamentDto dto);
    Task<TournamentDto> UpdateStatusAsync(int id, TournamentStatus status);
    Task DeleteTournamentAsync(int id);
    Task<List<RaceDto>> GetRacesByTournamentAsync(int tournamentId);
}
