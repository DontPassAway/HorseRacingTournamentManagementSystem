using HorseRacing.Domain.Entities;

namespace HorseRacing.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
