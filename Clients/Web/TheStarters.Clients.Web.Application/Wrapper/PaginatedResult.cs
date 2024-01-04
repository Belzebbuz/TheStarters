namespace TheStarters.Clients.Web.Application.Wrapper
{
    public class PaginatedResult<T> : Result
    {
        public PaginatedResult(List<T> data)
        {
            Data = data ?? new();
        }

        public List<T> Data { get; set; } = new();

        internal PaginatedResult(bool succeeded, List<T>? data = null, List<string>? messages = null, int count = 0, int page = 1, int pageSize = 10)
        {
            Data = data ?? new();
            CurrentPage = page;
            Succeeded = succeeded;
            ItemsPerPage = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Messages = messages ?? new();
        }

        public static PaginatedResult<T> Failure(List<string> messages)
        {
            return new PaginatedResult<T>(false, default, messages);
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
            return new PaginatedResult<T>(true, data, null, count, page, pageSize);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }
        public int ItemsPerPage { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}