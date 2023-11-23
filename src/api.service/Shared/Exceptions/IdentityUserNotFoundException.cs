using api.building.Exceptions;

namespace api.service.Shared.Exceptions
{
    public class IdentityUserNotFoundException : CustomException
    {
        public IdentityUserNotFoundException(string emailOrUserName) : base($"User with email or username: '{emailOrUserName}' not found.") { }
        public IdentityUserNotFoundException(Guid id): base($"User with id: '{id}' not found.") { }
    }
}
