using api.service.Shared.Models;
using api.service.Shared.Models.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Security.Claims;
using api.building.CQRS.Commands;
using api.service.Security;

namespace api.service.Identity.Features.GeneratingJwtToken.v1
{
    public record GenerateJwtToken(ApplicationUser User, string RefreshToken) : ICommand<GenerateJwtTokenResponse>;

    public class GenerateJwtTokenHandler : ICommandHandler<GenerateJwtToken, GenerateJwtTokenResponse>
    {
        private readonly ILogger<GenerateJwtTokenHandler> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;

        public GenerateJwtTokenHandler(UserManager<ApplicationUser> userManager,
            JwtService jwtService,
            ILogger<GenerateJwtTokenHandler> logger)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<GenerateJwtTokenResponse> Handle(GenerateJwtToken request, CancellationToken cancellationToken)
        {
            var identityUser = request.User;

            var allClaims = await GetClaimsAsync(request.User.UserName!);
            var fullName = $"{identityUser.FirstName} {identityUser.LastName}";

            var tokenResult = _jwtService.GenerateJwtToken(
                identityUser.UserName!,
                identityUser.Email!,
                identityUser.Id.ToString(),
                identityUser.EmailConfirmed || identityUser.PhoneNumberConfirmed,
                fullName,
                request.RefreshToken,
                allClaims.UserClaims.ToImmutableList(),
                allClaims.Roles.ToImmutableList(),
                allClaims.PermissionClaims.ToImmutableList()
            );

            _logger.LogInformation("access-token generated, \n: {AccessToken}", tokenResult.AccessToken);

            return new GenerateJwtTokenResponse(tokenResult.AccessToken, tokenResult.ExpireAt);
        }

        public async Task<(IList<Claim> UserClaims, IList<string> Roles, IList<string> PermissionClaims)> GetClaimsAsync(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            var userClaims = (await _userManager.GetClaimsAsync(appUser!))
                .Where(x => x.Type != CustomClaimTypes.Permission)
                .ToList();
            var roles = await _userManager.GetRolesAsync(appUser);

            var permissions = (await _userManager.GetClaimsAsync(appUser))
                .Where(x => x.Type == CustomClaimTypes.Permission)
                ?.Select(x => x.Value)
                .ToList();

            return (UserClaims: userClaims, Roles: roles, PermissionClaims: permissions)!;
        }
    }
}
