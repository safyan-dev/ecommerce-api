using api.building.CQRS.Commands;
using Microsoft.AspNetCore.Http;

namespace api.service.Identity.Features.GettingClaims.v1;

public record GetClaims : IQuery<GetClaimsResponse> { }

public class GetClaimsQueryHandler : IQueryHandler<GetClaims, GetClaimsResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetClaimsQueryHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<GetClaimsResponse> Handle(GetClaims request, CancellationToken cancellationToken)
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims.Select(x => new ClaimDto { Type = x.Type, Value = x.Value })!;

        return Task.FromResult(new GetClaimsResponse(claims));
    }
}
