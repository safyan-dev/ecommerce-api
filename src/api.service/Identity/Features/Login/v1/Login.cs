using api.service.Identity.Exceptions;
using api.service.Identity.Features.GeneratingJwtToken.v1;
using api.service.Identity.Features.GeneratingRefreshToken.v1;
using api.service.Shared.Data;
using api.service.Shared.Models;
using api.service.Shared.Models.IdentityModels;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using api.building.CQRS.Commands;
using api.building.Exceptions;
using api.service.Security;

namespace api.service.Identity.Features.Login.v1;

public record Login(string UserNameOrEmail, string Password, bool Remember) : ICommand<LoginResponse>;

internal class LoginValidator : AbstractValidator<Login>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("UserNameOrEmail cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("password cannot be empty");
    }
}

internal class LoginHandler : ICommandHandler<Login, LoginResponse>
{
    private readonly IMediator _commandProcessor;
    private readonly JwtService _jwtService;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<LoginHandler> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApiDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginHandler(
        UserManager<ApplicationUser> userManager,
        IMediator commandProcessor,
        JwtService jwtService,
        IOptions<JwtOptions> jwtOptions,
        SignInManager<ApplicationUser> signInManager,
        ApiDbContext context,
        ILogger<LoginHandler> logger
    )
    {
        _userManager = userManager;
        _commandProcessor = commandProcessor;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _context = context;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(Login request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var identityUser =
            await _userManager.FindByNameAsync(request.UserNameOrEmail)
            ?? await _userManager.FindByEmailAsync(request.UserNameOrEmail);

        ArgumentNullException.ThrowIfNull(nameof(identityUser), request.UserNameOrEmail);

        // instead of PasswordSignInAsync, we use CheckPasswordSignInAsync because we don't want set cookie, instead we use JWT
        var signinResult = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, false);

        if (signinResult.IsNotAllowed)
        {
            if (!await _userManager.IsEmailConfirmedAsync(identityUser))
                throw new EmailNotConfirmedException(identityUser.Email!);

            if (!await _userManager.IsPhoneNumberConfirmedAsync(identityUser))
                throw new PhoneNumberNotConfirmedException(identityUser.PhoneNumber!);
        }
        else if (signinResult.IsLockedOut)
        {
            throw new UserLockedException(identityUser.Id.ToString());
        }
        else if (signinResult.RequiresTwoFactor)
        {
            throw new RequiresTwoFactorException("Require two factor authentication.");
        }
        else if (!signinResult.Succeeded)
        {
            throw new PasswordIsInvalidException("Password is invalid.");
        }

        var refreshToken = (
            await _commandProcessor.Send(new GenerateRefreshToken(identityUser.Id), cancellationToken)
        ).RefreshToken;

        var accessToken = await _commandProcessor.Send(
            new GenerateJwtToken(identityUser, refreshToken.Token),
            cancellationToken
        );

        if (string.IsNullOrWhiteSpace(accessToken.Token))
            throw new CustomException("Generate access token failed.");

        _logger.LogInformation("User with ID: {ID} has been authenticated", identityUser.Id);

        if (_jwtOptions.CheckRevokedAccessTokens)
        {
            await _context.Set<AccessToken>().AddAsync(
                    new AccessToken
                    {
                        UserId = identityUser.Id,
                        Token = accessToken.Token,
                        CreatedAt = DateTime.Now,
                        ExpiredAt = accessToken.ExpireAt
                    },cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        // we can don't return value from command and get token from a short term session in our request with `TokenStorageService`
        return new LoginResponse(identityUser, accessToken.Token, refreshToken.Token);
    }
}
