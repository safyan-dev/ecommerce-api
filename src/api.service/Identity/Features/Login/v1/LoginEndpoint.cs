using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace api.service.Identity.Features.Login.v1;

public static class LoginEndpoint
{
    public static RouteHandlerBuilder MapLoginUserEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/login", LoginUser)
            .AllowAnonymous()
            .Produces<LoginResponse>()
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status500InternalServerError)
            .Produces<StatusCodeProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation => new(operation) { Summary = "Login User", Description = "Login User" })
            .WithDisplayName("Login User.")
            .WithName("Login")
            .MapToApiVersion(1.0);
    }

    private static async Task<IResult> LoginUser(
        LoginRequest request,
        IMediator commandProcessor,
        CancellationToken cancellationToken
    )
    {
        var command = new Login(request.UserNameOrEmail, request.Password, request.Remember);

        var result = await commandProcessor.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
