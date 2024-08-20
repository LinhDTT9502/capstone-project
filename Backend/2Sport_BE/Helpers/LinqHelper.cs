using System.Linq.Expressions;

namespace _2Sport_BE.Helpers
{
    public static class LinqHelper
    {
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string sortBy, bool isAscending)
        {
            if (String.IsNullOrWhiteSpace(sortBy))
            {
                return source;
            }
            var param = Expression.Parameter(typeof(T), "item");

            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

            if (isAscending)
            {
                return source.OrderBy<T, object>(sortExpression);
            }
            return source.OrderByDescending<T, object>(sortExpression);
        }


    }
}
