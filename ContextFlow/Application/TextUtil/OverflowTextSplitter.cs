namespace ContextFlow.Application.TextUtil;

using ContextFlow.Domain;

public class OverflowTextSplitter
{

    private readonly List<string> SplitIdentifierHierarchy;
    private LLMTokenizer Tokenizer;

    public OverflowTextSplitter(LLMTokenizer tokenizer, List<string> splitIdentifierHierarchy)
    {
        Tokenizer = tokenizer;
        if (splitIdentifierHierarchy.Count == 0)
        {
            throw new InvalidDataException("SplitIdentifierHierarchy must have at least one value");
        }
        SplitIdentifierHierarchy = splitIdentifierHierarchy;
    }

    public List<string> Split(string input, int maxStringTokens)
    {
        return Split(input, maxStringTokens, SplitIdentifierHierarchy[0]);
    }

    public List<string> Split(string input, int maxStringTokens, string splitIdentifier)
    {

        if (input.Contains(splitIdentifier))
        {
            return SplitTextInMiddle(input, splitIdentifier);
        }
        else
        {
            int index = SplitIdentifierHierarchy.IndexOf(splitIdentifier);
            if (index + 1 < SplitIdentifierHierarchy.Count)
            {
                string nextSplitIdentifier = SplitIdentifierHierarchy[index + 1];
                return Split(input, maxStringTokens, nextSplitIdentifier);
            }
            else
            {
                // Out of range in the hierarchy
                return new List<string> { input };
            }
        }
    }

    public List<string> SplitTextInMiddle(string input, string splitIdentifier)
    {
        int num = Tokenizer.CountTokens(input);
        int middleIndex = num;

        List<string> splitText = new List<string>
        {
            input.Substring(0, middleIndex),
            input.Substring(middleIndex + splitIdentifier.Length)
        };

        return splitText;
    }

    public static int FindClosestSubStrToMiddle(string input, string substr)
    {
        var commaIndices = input.Select((c, i) => input.Substring(i).StartsWith(substr) ? i : -1).Where(i => i >= 0).ToList();

        if (!commaIndices.Any())
            return -1;

        int middleIndex = input.Length / 2;

        return commaIndices.OrderBy(idx => Math.Abs(idx - middleIndex)).First();
    }
}
