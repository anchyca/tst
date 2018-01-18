using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadLocker.Products.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static Task<List<TSource>> ToListAsync<TSource>(this IQueryable<TSource> source)
        {
            if (source is IDbAsyncEnumerable)
            {
                return System.Data.Entity.QueryableExtensions.ToListAsync(source);
            }
            else
            {
                return Task.FromResult(source.ToList());
            }
        }
        public static Task<TSource> FirstOrDefaultAsync<TSource>(this IQueryable<TSource> source)
        {
            if (source is IDbAsyncEnumerable)
            {
                return System.Data.Entity.QueryableExtensions.FirstOrDefaultAsync(source);
            }
            else
            {
                return Task.FromResult(source.FirstOrDefault());
            }
        }
    }
}
