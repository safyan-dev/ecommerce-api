using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace api.service.Identity.Features.GettingClaims.v1;

public static class GetClaimsEndpoint
{
    public static RouteHandlerBuilder MapGetClaimsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/claims", GetClaims)
            .RequireAuthorization()
            .Produces<GetClaimsResponse>()
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status401Unauthorized)
            .WithOpenApi(operation => new(operation) { Summary = "Getting User Claims", Description = "Getting User Claims" })
            .WithDisplayName("Get User claims");
    }

    private static async Task<IResult> GetClaims(IMediator queryProcessor, CancellationToken cancellationToken)
    {
        var result = await queryProcessor.Send(new GetClaims(), cancellationToken);

        return Results.Ok(result);
    }
}
