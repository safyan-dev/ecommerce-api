using api.service.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace ECommerce.Services.Identity.Identity.Features.Logout.v1;

public static class LogoutEndpoint
{
    public static RouteHandlerBuilder MapLogoutEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/logout", Logout)
            .Produces(StatusCodes.Status200OK)
            .RequireAuthorization()
            .WithName("logout")
            .WithOpenApi(operation => new(operation) { Summary = "Logout User", Description = "Logout User" })
            .WithDisplayName("Logout User.");
    }

    private static async Task<IResult> Logout(
        HttpContext httpContext,
        IOptions<JwtOptions> jwtOptions)
    {
        await httpContext.SignOutAsync();

        return Results.Ok();
    }
}
