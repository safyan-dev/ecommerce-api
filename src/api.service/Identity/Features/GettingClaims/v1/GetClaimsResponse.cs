namespace api.service.Identity.Features.GettingClaims.v1;

public record GetClaimsResponse(IEnumerable<ClaimDto> Claims);
