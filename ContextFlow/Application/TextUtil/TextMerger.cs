using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class TextMerger
{
    public abstract string Merge(List<string> inputs);
}
public class FunctionTextMerger : TextMerger
{
    // A property that holds the lambda/function to merge the list of strings.
    public Func<List<string>, string> MergeFunction { get; set; }

    public FunctionTextMerger(Func<List<string>, string> mergeFunction)
    {
        MergeFunction = mergeFunction;
    }

    public override string Merge(List<string> inputs)
    {
        // Use the provided lambda/function to merge the list of strings.
        return MergeFunction(inputs);
    }
}
