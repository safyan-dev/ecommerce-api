using MediatR;

namespace api.building.CQRS.Commands
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Unit> where TCommand : ICommand<Unit> { };
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>{ };
}
