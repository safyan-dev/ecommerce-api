using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class RequiresTwoFactorException : CustomException
{
    public RequiresTwoFactorException(string message)
        : base(message) { }
}
