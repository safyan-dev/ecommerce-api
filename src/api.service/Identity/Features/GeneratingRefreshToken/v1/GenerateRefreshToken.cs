using api.service.Identity.Dtos.v1;
using api.service.Identity.Identity.Features.RefreshingToken.v1;
using api.service.Shared.Data;
using api.service.Shared.Models.IdentityModels;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using api.building.CQRS.Commands;

namespace api.service.Identity.Features.GeneratingRefreshToken.v1;

public record GenerateRefreshToken(Guid UserId, string? Token = null) : ICommand<GenerateRefreshTokenResponse>;

public class GenerateRefreshTokenHandler : ICommandHandler<GenerateRefreshToken, GenerateRefreshTokenResponse>
{
    private readonly ApiDbContext _context;

    public GenerateRefreshTokenHandler(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<GenerateRefreshTokenResponse> Handle(GenerateRefreshToken request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(GenerateRefreshToken));

        var refreshToken = await _context
            .Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.UserId == request.UserId && rt.Token == request.Token, cancellationToken);

        if (refreshToken == null)
        {
            var token = RefreshToken.GetRefreshToken();

            refreshToken = new RefreshToken
            {
                UserId = request.UserId,
                Token = token,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(1)
            };

            await _context.Set<RefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            if (!refreshToken.IsRefreshTokenValid())
                throw new InvalidRefreshTokenException(refreshToken);

            var token = RefreshToken.GetRefreshToken();

            refreshToken.Token = token;
            refreshToken.ExpiredAt = DateTime.Now;
            refreshToken.CreatedAt = DateTime.Now.AddDays(10);

            _context.Set<RefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // remove old refresh tokens from user
        // we could also maintain them on the database with changing their revoke date
        await RemoveOldRefreshTokens(request.UserId);

        return new GenerateRefreshTokenResponse(
            new RefreshTokenDto
            {
                Token = refreshToken.Token,
                CreatedAt = refreshToken.CreatedAt,
                ExpireAt = refreshToken.ExpiredAt,
                UserId = refreshToken.UserId,
                CreatedByIp = refreshToken.CreatedByIp,
                IsActive = refreshToken.IsActive,
                IsExpired = refreshToken.IsExpired,
                IsRevoked = refreshToken.IsRevoked,
                RevokedAt = refreshToken.RevokedAt
            }
        );
    }

    private Task RemoveOldRefreshTokens(Guid userId, long? ttlRefreshToken = null)
    {
        var refreshTokens = _context
            .Set<RefreshToken>()
            .Where(rt => rt.UserId == userId);

        refreshTokens.ToList().RemoveAll(x => !x.IsRefreshTokenValid(ttlRefreshToken));

        return _context.SaveChangesAsync();
    }
}
