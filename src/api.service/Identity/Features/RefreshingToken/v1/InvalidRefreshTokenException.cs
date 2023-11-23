using api.service.Shared.Models.IdentityModels;
using api.building.Exceptions;

namespace api.service.Identity.Identity.Features.RefreshingToken.v1;

public class InvalidRefreshTokenException : CustomException
{
    public InvalidRefreshTokenException(RefreshToken? refreshToken)
        : base($"refresh token {refreshToken?.Token} is invalid!") { }
}
