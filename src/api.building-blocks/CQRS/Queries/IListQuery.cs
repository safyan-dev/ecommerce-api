using api.building.CQRS.Commands;

namespace api.building.CQRS.Queries
{
    public record FilterModel(string FieldName, string Comparision, string FieldValue);
    public interface IPageRequest
    {
        IList<string>? Includes { get; init; }
        IList<FilterModel>? Filters { get; init; }
        IList<string>? Sorts { get; init; }
        int Page { get; init; }
        int PageSize { get; init; }
    }

    public interface IListQuery<TResponse> : IPageRequest, IQuery<TResponse>
        where TResponse : notnull{ }

    public record ListQuery<TResponse> : IListQuery<TResponse>
        where TResponse : notnull
    {
        public IList<string>? Includes { get; init; }
        public IList<FilterModel>? Filters { get; init; }
        public IList<string>? Sorts { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }

    public record ListResultModel<T>(List<T> Items, long TotalItems, int Page, int PageSize) where T : notnull
    {
        public static ListResultModel<T> Empty => new(Enumerable.Empty<T>().ToList(), 0, 0, 0);

        public static ListResultModel<T> Create(List<T> items, long totalItems = 0, int page = 1, int pageSize = 20)
        {
            return new ListResultModel<T>(items, totalItems, page, pageSize);
        }

        public ListResultModel<U> Map<U>(Func<T, U> map)
        {
            return ListResultModel<U>.Create(Items.Select(map).ToList(), TotalItems, Page, PageSize);
        }
    }
}
