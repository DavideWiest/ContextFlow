namespace ContextFlow.Application.TextUtil;

public abstract class TextSplitter
{
    public abstract IEnumerable<string> Split(string text);
}

public class FunctionTextSplitter : TextSplitter
{
    // A property that holds the lambda/function to split the text.
    public Func<string, IEnumerable<string>> SplitFunction { get; set; }

    public FunctionTextSplitter(Func<string, IEnumerable<string>> splitFunction)
    {
        SplitFunction = splitFunction;
    }

    public override IEnumerable<string> Split(string text)
    {
        // Use the provided lambda/function to split the text.
        return SplitFunction(text);
    }
}