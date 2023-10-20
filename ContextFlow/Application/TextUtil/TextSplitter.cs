using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class TextSplitter
{
    public abstract List<string> Split(string text);
}

public class FunctionTextSplitter : TextSplitter
{
    // A property that holds the lambda/function to split the text.
    public Func<string, List<string>> SplitFunction { get; set; }

    public FunctionTextSplitter(Func<string, List<string>> splitFunction)
    {
        SplitFunction = splitFunction;
    }

    public override List<string> Split(string text)
    {
        // Use the provided lambda/function to split the text.
        return SplitFunction(text);
    }
}