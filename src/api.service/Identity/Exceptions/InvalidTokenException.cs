using System.Security.Claims;
using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class InvalidTokenException : CustomException
{
    public InvalidTokenException(ClaimsPrincipal? claimsPrincipal)
        : base("access_token is invalid!") { }
}
