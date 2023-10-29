
namespace ContextFlow.Application.Result.Actions;

internal class ActionsUtil
{
    public static (IEnumerable<T> Passed, IEnumerable<T> Failed) Partition<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();

        foreach (var item in source)
        {
            if (predicate(item))
            {
                trueList.Add(item);
            }
            else
            {
                falseList.Add(item);
            }
        }

        return (trueList, falseList);
    }

    public static async Task<(IEnumerable<T> Passed, IEnumerable<T> Failed)> PartitionAsync<T>(IEnumerable<T> source, Func<T, Task<bool>> asyncPredicate)
    {
        var trueList = new List<T>();
        var falseList = new List<T>();
        var tasks = new List<Task>();

        foreach (var item in source)
        {
            tasks.Add(PartitionItemAsync(item, asyncPredicate, trueList, falseList));
        }

        await Task.WhenAll(tasks);

        return (trueList, falseList);
    }

    private static async Task PartitionItemAsync<T>(T item, Func<T, Task<bool>> asyncPredicate, List<T> trueList, List<T> falseList)
    {
        var result = await asyncPredicate(item);
        if (result)
        {
            lock (trueList)
            {
                trueList.Add(item);
            }
        }
        else
        {
            lock (falseList)
            {
                falseList.Add(item);
            }
        }
    }
}
