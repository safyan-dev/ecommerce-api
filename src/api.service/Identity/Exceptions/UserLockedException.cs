using api.building.Exceptions;

namespace api.service.Identity.Exceptions;

public class UserLockedException : CustomException
{
    public UserLockedException(string userId)
        : base($"userId '{userId}' has been locked.") { }
}
