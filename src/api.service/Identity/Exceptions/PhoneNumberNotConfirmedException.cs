using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class PhoneNumberNotConfirmedException : CustomException
{
    public PhoneNumberNotConfirmedException(string phone)
        : base($"The phone number '{phone}' is not confirmed yet.") { }
}
