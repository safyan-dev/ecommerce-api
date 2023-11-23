using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class PasswordIsInvalidException : CustomException
{
    public PasswordIsInvalidException(string message)
        : base(message) { }
}
