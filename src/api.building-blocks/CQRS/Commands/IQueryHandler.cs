using MediatR;

namespace api.building.CQRS.Commands
{
    public interface IQueryHandler<TQuery>: IRequestHandler<TQuery,Unit>
        where TQuery : IQuery<Unit>{};

    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
        where TResponse : notnull { };
}
