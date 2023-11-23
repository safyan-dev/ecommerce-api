using MediatR;

namespace api.building.CQRS.Commands
{
    public interface IQuery : IRequest<Unit>
    { }
    public interface IQuery<T> : IRequest<T>
    { }
}
