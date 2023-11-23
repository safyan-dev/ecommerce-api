using api.service.Shared.Data;
using api.service.Shared.Models.IdentityModels;
using Microsoft.EntityFrameworkCore;
using api.building.CQRS.Commands;

namespace api.service.Identity.Features.GettingRefreshTokenValidity.v1;

public record GetRefreshTokenValidity(Guid UserId, string RefreshToken) : IQuery<bool>;

public class GetRefreshTokenValidityQueryHandler : IQueryHandler<GetRefreshTokenValidity, bool>
{
    private readonly ApiDbContext _context;

    public GetRefreshTokenValidityQueryHandler(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(GetRefreshTokenValidity request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context
            .Set<RefreshToken>()
            .FirstOrDefaultAsync(
                rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken,
                cancellationToken
            );

        if (refreshToken == null)
        {
            return false;
        }

        if (!refreshToken.IsRefreshTokenValid())
            return false;

        return true;
    }
}
