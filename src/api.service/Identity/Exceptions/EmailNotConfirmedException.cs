using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

// https://stackoverflow.com/questions/36283377/http-status-for-email-not-verified
public class EmailNotConfirmedException : CustomException
{
    public EmailNotConfirmedException(string email)
        : base($"Email not confirmed for email address `{email}`")
    {
        Email = email;
    }

    public string Email { get; }
}
