using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class TextMerger
{
    public abstract string Merge(IEnumerable<string> inputs);
}
public class FunctionTextMerger : TextMerger
{
    // A property that holds the lambda/function to merge the list of strings.
    public Func<IEnumerable<string>, string> MergeFunction { get; set; }

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
