using HorseRacing.Application.DTOs.Notifications;
using HorseRacing.Application.Interfaces.Repositories;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Entities;
using HorseRacing.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HorseRacing.Infrastructure.Services;

/// <summary>
/// Option A: Aggregate notifications in-memory from existing tables (no new DB table needed).
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IGenericRepository<JockeyInvitation> _invitationRepo;
    private readonly IGenericRepository<Bet> _betRepo;
    private readonly IGenericRepository<Registration> _registrationRepo;
    private readonly IGenericRepository<JockeyProfile> _jockeyProfileRepo;

    public NotificationService(
        IGenericRepository<JockeyInvitation> invitationRepo,
        IGenericRepository<Bet> betRepo,
        IGenericRepository<Registration> registrationRepo,
        IGenericRepository<JockeyProfile> jockeyProfileRepo)
    {
        _invitationRepo = invitationRepo;
        _betRepo = betRepo;
        _registrationRepo = registrationRepo;
        _jockeyProfileRepo = jockeyProfileRepo;
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(int userId)
    {
        var notifications = new List<NotificationDto>();
        int idCounter = 1;

        // ── 1. Jockey: lời mời nhận được ────────────────────────────────────
        var jockeyProfile = await _jockeyProfileRepo.FirstOrDefaultAsync(j => j.UserId == userId);
        if (jockeyProfile != null)
        {
            var invitations = await _invitationRepo.Query()
                .Include(i => i.Horse)
                .Include(i => i.HorseOwner).ThenInclude(o => o.User)
                .Where(i => i.JockeyId == jockeyProfile.Id)
                .OrderByDescending(i => i.InvitedAt)
                .Take(20)
                .ToListAsync();

            foreach (var inv in invitations)
            {
                var isNew = inv.Status == JockeyInvitationStatus.Pending;
                notifications.Add(new NotificationDto
                {
                    Id = idCounter++,
                    Type = "JockeyInvitation",
                    Title = isNew ? "Lời mời cưỡi ngựa mới" : $"Lời mời đã {(inv.Status == JockeyInvitationStatus.Accepted ? "chấp nhận" : "từ chối")}",
                    Message = $"Chủ ngựa '{inv.HorseOwner.User.FullName}' mời bạn cưỡi ngựa '{inv.Horse.Name}'.",
                    IsRead = !isNew,
                    CreatedAt = inv.InvitedAt,
                    ReferenceId = inv.Id,
                    ReferenceType = "JockeyInvitation"
                });
            }
        }

        // ── 2. Spectator: kết quả cược ──────────────────────────────────────
        var resolvedBets = await _betRepo.Query()
            .Include(b => b.Race)
            .Include(b => b.PredictedHorse)
            .Where(b => b.SpectatorUserId == userId && b.Status != BetStatus.Pending)
            .OrderByDescending(b => b.ResolvedAt)
            .Take(20)
            .ToListAsync();

        foreach (var bet in resolvedBets)
        {
            notifications.Add(new NotificationDto
            {
                Id = idCounter++,
                Type = "BetResult",
                Title = bet.Status == BetStatus.Won ? "Bạn đã thắng cược!" : "Cược thua",
                Message = bet.Status == BetStatus.Won
                    ? $"Chúc mừng! Dự đoán của bạn về ngựa '{bet.PredictedHorse.Name}' trong cuộc đua '{bet.Race.Name}' chính xác!"
                    : $"Dự đoán ngựa '{bet.PredictedHorse.Name}' trong '{bet.Race.Name}' không chính xác.",
                IsRead = true,
                CreatedAt = bet.ResolvedAt ?? bet.PlacedAt,
                ReferenceId = bet.Id,
                ReferenceType = "Bet"
            });
        }

        // ── 3. HorseOwner: trạng thái đăng ký ──────────────────────────────
        var regs = await _registrationRepo.Query()
            .Include(r => r.HorseOwner)
            .Include(r => r.Race)
            .Include(r => r.Horse)
            .Where(r => r.HorseOwner.UserId == userId &&
                        (r.Status == RegistrationStatus.Approved || r.Status == RegistrationStatus.Rejected))
            .OrderByDescending(r => r.RegisteredAt)
            .Take(20)
            .ToListAsync();

        foreach (var reg in regs)
        {
            bool approved = reg.Status == RegistrationStatus.Approved;
            notifications.Add(new NotificationDto
            {
                Id = idCounter++,
                Type = "RegistrationStatus",
                Title = approved ? "Đăng ký được duyệt" : "Đăng ký bị từ chối",
                Message = approved
                    ? $"Ngựa '{reg.Horse.Name}' đã được duyệt tham gia '{reg.Race.Name}' ở làn {reg.LaneNumber}."
                    : $"Ngựa '{reg.Horse.Name}' bị từ chối tham gia '{reg.Race.Name}'. Lý do: {reg.RejectionReason}",
                IsRead = true,
                CreatedAt = reg.RegisteredAt,
                ReferenceId = reg.Id,
                ReferenceType = "Registration"
            });
        }

        return notifications.OrderByDescending(n => n.CreatedAt).ToList();
    }
}
