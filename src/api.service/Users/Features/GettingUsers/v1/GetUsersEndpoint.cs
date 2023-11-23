using api.service.Identity.Features.Login.v1;
using Ardalis.GuardClauses;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace api.service.Users.Features.GettingUsers.v1;

public static class GetUsersEndpoint
{
    public static RouteHandlerBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/GetUsers", GetUsers)
            .AllowAnonymous()
            .Produces<LoginResponse>()
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status500InternalServerError)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new(operation) { Summary = "Get Users", Description = "Get Users" })
            .WithDisplayName("Get Users.")
            .WithName("GetUsers")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> GetUsers([FromBody] GetUsersRequest? request,
        IMediator _queryProcessor,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var result = await _queryProcessor.Send(
            new GetUsers(),
            cancellationToken
        );

        return Results.Ok(result);
    }
}
