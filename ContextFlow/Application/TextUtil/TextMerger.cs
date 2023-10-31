namespace ContextFlow.Application.TextUtil;

/// <summary>
/// Merges an enumerable of strings
/// </summary>
public abstract class TextMerger
{
    public abstract string Merge(IEnumerable<string> inputs);
}


public class FunctionTextMerger : TextMerger
{
    /// <summary>
    /// A property that holds the lambda/function to merge the list of strings.
    /// </summary>
    public Func<IEnumerable<string>, string> MergeFunction { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mergeFunction">A property that holds the lambda/function to merge the list of strings.</param>
    public FunctionTextMerger(Func<IEnumerable<string>, string> mergeFunction)
    {
        MergeFunction = mergeFunction;
    }

    public override string Merge(IEnumerable<string> inputs)
    {
        // Use the provided lambda/function to merge the list of strings.
        return MergeFunction(inputs);
    }
}
