using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        // by Stephen Toub ################################################################################################
        //https://blogs.msdn.microsoft.com/pfxteam/2012/03/05/implementing-a-simple-foreachasync-part-2/ 

        public async static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> operation) =>
            await Task.WhenAll(
                from item
                in source
                select Task.Run(async () =>
                    await operation(item).ConfigureAwait(false)));

        public async static Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> operation, int degreeOfParallelism) =>
            await Task.WhenAll(
                from partition
                in Partitioner.Create(source).GetPartitions(degreeOfParallelism)
                select Task.Run(async delegate
                {
                    using (partition)
                        while (partition.MoveNext())
                            await operation(partition.Current).ConfigureAwait(false);
                }));
        //#################################################################################################################
    }
}