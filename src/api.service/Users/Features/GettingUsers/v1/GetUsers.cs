using api.service.Shared.Models.IdentityModels;
using api.service.Users.Dtos.v1;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using api.building.CQRS.Queries;
using api.building.CQRS.Commands;

namespace api.service.Users.Features.GettingUsers.v1;

public record GetUsers : ListQuery<GetUsersResponse>;

public class GetUsersValidator : AbstractValidator<GetUsers>
{
    public GetUsersValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page should at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageSize should at least greater than or equal to 1.");
    }
}

public class GetUsersHandler : IQueryHandler<GetUsers, GetUsersResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public GetUsersHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<GetUsersResponse> Handle(GetUsers request, CancellationToken cancellationToken)
    {
        var customer = await _userManager.Users.Select(e=> new IdentityUserDto
        {
            Id = e.Id,
            CreatedAt = e.CreatedAt,
            Email = e.Email,
            FirstName = e.FirstName,
            LastName = e.LastName,
            UserName = e.UserName
        }).ToListAsync();

        return new GetUsersResponse(customer);
    }
}
