using HorseRacing.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HorseRacing.API.Filters;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
    public AuthorizeRolesAttribute(params UserRole[] roles)
    {
        Roles = string.Join(",", roles.Select(r => r.ToString()));
    }
}
