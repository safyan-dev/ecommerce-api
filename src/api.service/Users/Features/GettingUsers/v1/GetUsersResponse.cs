using api.service.Users.Dtos.v1;

namespace api.service.Users.Features.GettingUsers.v1;

public record GetUsersResponse(List<IdentityUserDto> IdentityUsers);
