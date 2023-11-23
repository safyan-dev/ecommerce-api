namespace api.service.Identity.Features.Login.v1;

public record LoginRequest(string UserNameOrEmail, string Password, bool Remember);