using AutoMapper;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.DTOs.Bets;
using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Application.DTOs.JockeyInvitations;
using HorseRacing.Application.DTOs.Prizes;
using HorseRacing.Application.DTOs.RaceAssignments;
using HorseRacing.Application.DTOs.RaceResults;
using HorseRacing.Application.DTOs.Races;
using HorseRacing.Application.DTOs.Registrations;
using HorseRacing.Application.DTOs.RefereeReports;
using HorseRacing.Application.DTOs.Tournaments;
using HorseRacing.Domain.Entities;

namespace HorseRacing.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserProfileDto>();

        // Horse
        CreateMap<Horse, HorseDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner.User.FullName));

        // Tournament
        CreateMap<Tournament, TournamentDto>()
            .ForMember(d => d.TotalRaces, opt => opt.MapFrom(s => s.Races.Count));

        // Race
        CreateMap<Race, RaceDto>()
            .ForMember(d => d.TournamentName, opt => opt.MapFrom(s => s.Tournament.Name))
            .ForMember(d => d.TotalRegistrations, opt => opt.MapFrom(s => s.Registrations.Count));

        // Registration
        CreateMap<Registration, RegistrationDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race.Name))
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Horse.Name))
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner.User.FullName))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s => s.Jockey != null ? s.Jockey.User.FullName : null));

        // RaceResult
        CreateMap<RaceResult, RaceResultDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race.Name))
            .ForMember(d => d.HorseId, opt => opt.MapFrom(s => s.Registration.HorseId))
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Registration.Horse.Name))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s =>
                s.Registration.Jockey != null ? s.Registration.Jockey.User.FullName : null));

        // Bet
        CreateMap<Bet, BetDto>()
            .ForMember(d => d.SpectatorName, opt => opt.MapFrom(s => s.SpectatorUser.FullName))
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race.Name))
            .ForMember(d => d.PredictedHorseName, opt => opt.MapFrom(s => s.PredictedHorse.Name));

        // Prize
        CreateMap<Prize, PrizeDto>()
            .ForMember(d => d.TournamentName, opt => opt.MapFrom(s => s.Tournament.Name));

        // RefereeReport
        CreateMap<RefereeReport, RefereeReportDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race.Name))
            .ForMember(d => d.RefereeName, opt => opt.MapFrom(s => s.RefereeUser.FullName));

        // JockeyInvitation
        CreateMap<JockeyInvitation, JockeyInvitationDto>()
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Horse.Name))
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner.User.FullName))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s => s.Jockey.User.FullName))
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null));

        // RaceAssignment
        CreateMap<RaceAssignment, RaceAssignmentDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race.Name))
            .ForMember(d => d.RefereeName, opt => opt.MapFrom(s => s.RefereeUser.FullName));
    }
}
