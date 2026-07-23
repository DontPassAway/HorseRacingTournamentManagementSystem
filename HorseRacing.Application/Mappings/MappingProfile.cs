using AutoMapper;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.DTOs.Bets;
using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Application.DTOs.JockeyInvitations;
using HorseRacing.Application.DTOs.Prizes;
using HorseRacing.Application.DTOs.Profiles;
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
        // Fix: AutoMapper 12 requires DisableConstructorMapping for C# records with positional constructors
        DisableConstructorMapping();

        // User
        CreateMap<User, UserProfileDto>();

        // Jockey Profile
        CreateMap<JockeyProfile, JockeyProfileDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));

        CreateMap<UpdateJockeyProfileDto, JockeyProfile>();

        // Horse Owner (Sử dụng HorseOwner thay cho HorseOwnerProfile để fix CS0246)
        CreateMap<HorseOwner, HorseOwnerProfileDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));

        CreateMap<UpdateHorseOwnerProfileDto, HorseOwner>();

        // Horse
        CreateMap<Horse, HorseDto>()
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner != null && s.HorseOwner.User != null ? s.HorseOwner.User.FullName : string.Empty));

        // Tournament
        CreateMap<Tournament, TournamentDto>()
            .ForMember(d => d.TotalRaces, opt => opt.MapFrom(s => s.Races != null ? s.Races.Count : 0));

        // Race
        CreateMap<Race, RaceDto>()
            .ForMember(d => d.TournamentName, opt => opt.MapFrom(s => s.Tournament != null ? s.Tournament.Name : null))
            .ForMember(d => d.TotalRegistrations, opt => opt.MapFrom(s => s.Registrations != null ? s.Registrations.Count : 0));

        // Registration
        CreateMap<Registration, RegistrationDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null))
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Horse != null ? s.Horse.Name : null))
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner != null && s.HorseOwner.User != null ? s.HorseOwner.User.FullName : null))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s => s.Jockey != null && s.Jockey.User != null ? s.Jockey.User.FullName : null));

        // RaceResult
        CreateMap<RaceResult, RaceResultDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null))
            .ForMember(d => d.HorseId, opt => opt.MapFrom(s => s.Registration != null ? s.Registration.HorseId : 0))
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Registration != null && s.Registration.Horse != null ? s.Registration.Horse.Name : null))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s =>
                s.Registration != null && s.Registration.Jockey != null && s.Registration.Jockey.User != null ? s.Registration.Jockey.User.FullName : null));

        // Bet
        CreateMap<Bet, BetDto>()
            .ForMember(d => d.SpectatorName, opt => opt.MapFrom(s => s.SpectatorUser != null ? s.SpectatorUser.FullName : null))
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null))
            .ForMember(d => d.PredictedHorseName, opt => opt.MapFrom(s => s.PredictedHorse != null ? s.PredictedHorse.Name : null));

        // Prize
        CreateMap<Prize, PrizeDto>()
            .ForMember(d => d.TournamentName, opt => opt.MapFrom(s => s.Tournament != null ? s.Tournament.Name : null));

        // RefereeReport
        CreateMap<RefereeReport, RefereeReportDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null))
            .ForMember(d => d.RefereeName, opt => opt.MapFrom(s => s.RefereeUser != null ? s.RefereeUser.FullName : null));

        // JockeyInvitation
        CreateMap<JockeyInvitation, JockeyInvitationDto>()
            .ForMember(d => d.HorseName, opt => opt.MapFrom(s => s.Horse != null ? s.Horse.Name : null))
            .ForMember(d => d.OwnerName, opt => opt.MapFrom(s => s.HorseOwner != null && s.HorseOwner.User != null ? s.HorseOwner.User.FullName : null))
            .ForMember(d => d.JockeyName, opt => opt.MapFrom(s => s.Jockey != null && s.Jockey.User != null ? s.Jockey.User.FullName : null))
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null));

        // RaceAssignment
        CreateMap<RaceAssignment, RaceAssignmentDto>()
            .ForMember(d => d.RaceName, opt => opt.MapFrom(s => s.Race != null ? s.Race.Name : null))
            .ForMember(d => d.RefereeName, opt => opt.MapFrom(s => s.RefereeUser != null ? s.RefereeUser.FullName : null));
    }
}