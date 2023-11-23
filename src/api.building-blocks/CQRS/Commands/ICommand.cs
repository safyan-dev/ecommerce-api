using MediatR;

namespace api.building.CQRS.Commands
{
    public interface ICommand: IRequest<Unit> { }
    public interface ICommand<T> : IRequest<T> { }
}
