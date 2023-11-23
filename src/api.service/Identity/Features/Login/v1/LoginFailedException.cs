using api.building.Exceptions;

namespace api.service.Identity.Features.Login.v1;

public class LoginFailedException : CustomException
{
    public LoginFailedException(string userNameOrEmail)
        : base($"Login failed for username: {userNameOrEmail}")
    {
        UserNameOrEmail = userNameOrEmail;
    }

    public string UserNameOrEmail { get; }
}
