using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.OpenIddict
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
        {
#if NET_CORE2
            List<T> results = new List<T>();

            IAsyncEnumerator<T> enumerator = items.GetAsyncEnumerator();

            while(await enumerator.MoveNextAsync())
            {
                results.Add(enumerator.Current);
            }

            await enumerator.DisposeAsync();

            return results;
#else
            List<T> results = new List<T>();

            await foreach (T item in items.WithCancellation(cancellationToken))
            { 
                results.Add(item);
            }

            return results;
#endif
        }
    }
}
