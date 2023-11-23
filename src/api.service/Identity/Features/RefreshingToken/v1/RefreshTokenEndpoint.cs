using api.service.Identity.Features.RefreshingToken.v1;
using api.service.Shared.Models.IdentityModels;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace api.service.Identity.Features.RefreshingToken.v1;

public static class RefreshTokenEndpoint
{
    public static RouteHandlerBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {

        return endpoints.MapPost("/refresh-token", GetRefreshToken)
            .RequireAuthorization()
            .Produces<RefreshTokenResponse>()
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            .WithName("RefreshToken")
            .WithDisplayName("Refresh Token")
            .WithMetadata(new SwaggerOperationAttribute("Refreshing Token", "Refreshing Token"));
    }

    private static async Task<IResult> GetRefreshToken(RefreshTokenRequest request, IMediator commandProcessor,CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);

        var result = await commandProcessor.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
