using HorseRacing.Shared.Constants;
using System.Security.Claims;

namespace HorseRacing.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(AppConstants.TokenClaimUserId);
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
