using api.service.Shared.Models.IdentityModels;
using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class RefreshTokenNotFoundException : CustomException
{
    public RefreshTokenNotFoundException(RefreshToken? refreshToken)
        : base("Refresh token not found.") { }
}
