using System.Security.Claims;
using api.service.Identity.Exceptions;
using api.service.Identity.Features.GeneratingJwtToken.v1;
using api.service.Identity.Features.GeneratingRefreshToken.v1;
using api.service.Shared.Exceptions;
using api.service.Shared.Models.IdentityModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using api.building.CQRS.Commands;
using api.service.Security;

namespace api.service.Identity.Features.RefreshingToken.v1;

public record RefreshTokenCommand(string AccessTokenData, string RefreshTokenData) : ICommand<RefreshTokenResponse>;

internal class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(v => v.AccessTokenData).NotEmpty();

        RuleFor(v => v.RefreshTokenData).NotEmpty();
    }
}

internal class RefreshTokenHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IMediator _commandProcessor;
    private readonly JwtService _jwtService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RefreshTokenHandler(JwtService jwtService,
        UserManager<ApplicationUser> userManager,
        IMediator commandProcessor)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _commandProcessor = commandProcessor;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        // invalid token/signing key was passed and we can't extract user claims
        var userClaimsPrincipal = _jwtService.GetPrincipalFromToken(request.AccessTokenData);

        if (userClaimsPrincipal is null)
            throw new InvalidTokenException(userClaimsPrincipal);

        var userId = userClaimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.NameId);

        var identityUser = await _userManager.FindByIdAsync(userId);

        if (identityUser == null)
            throw new IdentityUserNotFoundException(userId);

        var refreshToken = (await _commandProcessor.Send(new GenerateRefreshToken(identityUser.Id, request.RefreshTokenData),cancellationToken)).RefreshToken;

        var accessToken = await _commandProcessor.Send(new GenerateJwtToken(identityUser, refreshToken.Token),cancellationToken);

        return new RefreshTokenResponse(identityUser, accessToken.Token, refreshToken.Token);
    }
}
