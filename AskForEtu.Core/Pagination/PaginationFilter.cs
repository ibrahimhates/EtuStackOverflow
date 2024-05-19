namespace AskForEtu.Core.Pagination
{
    public static class PaginationFilter
    {
        public static IQueryable<T> ToPagging<T>(this IQueryable<T> query,int pageNumber,int pageSize)
        {
            var pagedList = query
                    .Skip((pageNumber - 1)*pageSize)
                    .Take(pageSize);

            return pagedList;
        }
    }
}
