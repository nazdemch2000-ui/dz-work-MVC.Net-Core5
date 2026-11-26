using System.Linq.Expressions;

namespace GameStore.Pages
{
    // Сторінка даних із застосованими пошуком, сортуванням і розбиттям на сторінки.
    public class PagedList<T> : List<T>
    {
        public PagedList(IQueryable<T> query, QueryOptions options)
        {
            Options = options;
            CurrentPage = options.CurrentPage;
            PageSize = options.PageSize;

            if (!string.IsNullOrEmpty(options.OrderPropertyName))
            {
                query = Order(query, options.OrderPropertyName, options.DescendingOrder);
            }
            if (!string.IsNullOrEmpty(options.SearchPropertyName) && !string.IsNullOrEmpty(options.SearchTerm))
            {
                query = Search(query, options.SearchPropertyName, options.SearchTerm);
            }

            TotalPages = (int)Math.Ceiling((decimal)query.Count() / PageSize);

            AddRange(query.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public QueryOptions Options { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        // Будує вираз x => x.Property.Contains(searchTerm), підтримує вкладені властивості ("Category.Name").
        private static IQueryable<T> Search(IQueryable<T> query, string propertyName, string searchTerm)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var body = Expression.Call(source, "Contains", Type.EmptyTypes, Expression.Constant(searchTerm, typeof(string)));
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return query.Where(lambda);
        }

        // Будує OrderBy/OrderByDescending за іменем властивості ("RetailPrice", "Category.Name").
        private static IQueryable<T> Order(IQueryable<T> query, string propertyName, bool desc)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var source = propertyName.Split('.').Aggregate((Expression)parameter, Expression.Property);
            var lambda = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), source.Type), source, parameter);
            return (IQueryable<T>)typeof(Queryable).GetMethods().Single(e =>
                    e.Name == (desc ? "OrderByDescending" : "OrderBy") &&
                    e.IsGenericMethodDefinition &&
                    e.GetGenericArguments().Length == 2 &&
                    e.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), source.Type)
                .Invoke(null, new object[] { query, lambda })!;
        }
    }
}
