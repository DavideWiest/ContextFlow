namespace ContextFlow.Application.TextUtil;

using ContextFlow.Infrastructure.Providers;

/// <summary>
/// A TextSplitter implementation that halves text based on a tokenizer until the maximum has been reached. 
/// It splits the text up hierarchically, meaning it will use the current identifier fully before splitting by the next one.
/// </summary>
public class HierarchichalTextSplitter : TextSplitter
{

    private readonly List<string> SplitIdentifierHierarchy = new();
    private readonly List<string> IdentifiersToAddToBeginnings = new();
    private readonly int MaxStringTokens;
    private LLMTokenizer Tokenizer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokenizer">The tokenizer used</param>
    /// <param name="maxStringTokens">The maximum tokens per text part</param>
    /// <param name="splitIdentifierHierarchy">An ordered list of identifers to split by.</param>
    /// <param name="identifiersToAddToBeginnings">A subset of the identifier-hierarchy, which denotes which identifiers will be added to the beginning of a text-part after splitting by that identifier</param>
    public HierarchichalTextSplitter(LLMTokenizer tokenizer, int maxStringTokens, List<string> splitIdentifierHierarchy, List<string> identifiersToAddToBeginnings)
    {
        Tokenizer = tokenizer;
        SplitIdentifierHierarchy.AddRange(splitIdentifierHierarchy);

        Validate();

        IdentifiersToAddToBeginnings.AddRange(identifiersToAddToBeginnings);
        MaxStringTokens = maxStringTokens;
    }

    private void Validate()
    {
        if (SplitIdentifierHierarchy.ToList().Count == 0)
        {
            throw new InvalidDataException("SplitIdentifierHierarchy must have at least one value");
        }
    }

    /// <summary>
    /// Splits the text up
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public override List<string> Split(string input)
    {
        return Split(input, 0);
    }

    protected List<string> Split(string input, int splitIdentifierIdx)
    {
        string splittableInput = input.StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]) ?
           input[SplitIdentifierHierarchy[splitIdentifierIdx].Length..] : input;

        if (!splittableInput.Contains(SplitIdentifierHierarchy[splitIdentifierIdx]))
        {
            return SplitWithNextIdentifier(input, splitIdentifierIdx);
        }

        List<string> substrs = new();

        var splittedinputList = SplitWithCurrentSplitIdentifier(input, splitIdentifierIdx);

        foreach (string str in splittedinputList)
        {
            if (Measure(str) > MaxStringTokens)
            {
                substrs.AddRange(Split(str, splitIdentifierIdx)); // will automatically call itself with the next identifier
            }
            else
            {
                substrs.Add(str);
            }
        }
        return substrs;
    }

    protected List<string> SplitWithNextIdentifier(string input, int splitIdentifierIdx)
    {
        if (splitIdentifierIdx + 1 < SplitIdentifierHierarchy.Count)
        {
            return Split(input, splitIdentifierIdx + 1);
        }
        else
        {
            throw new InvalidDataException("Cannot split string further with given split-identifiers. Add an empty string to the end of the identifier to ensure its split.");
        }
    }

    protected List<string> SplitWithCurrentSplitIdentifier(string input, int splitIdentifierIdx)
    {
        if (SplitIdentifierHierarchy[splitIdentifierIdx] == "")
        {
            return CharacterSplitByTokenNum(input).ToList();
        }
        bool inputStartsWithIdentifier = input.StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]);
        string splittableInput = inputStartsWithIdentifier ?
           input[SplitIdentifierHierarchy[splitIdentifierIdx].Length..] : input;

        int middleIndex = FindClosestSubStrToMiddle(splittableInput, splitIdentifierIdx);

        if (middleIndex == -1)
        {
            return new() { input };
        }

        string substr1 = string.Concat(inputStartsWithIdentifier && IdentifiersToAddToBeginnings.Contains(SplitIdentifierHierarchy[splitIdentifierIdx])
            ? SplitIdentifierHierarchy[splitIdentifierIdx] : "", input.AsSpan(0, middleIndex));

        string substr2 =
            (IdentifiersToAddToBeginnings.Contains(SplitIdentifierHierarchy[splitIdentifierIdx])
            ? SplitIdentifierHierarchy[splitIdentifierIdx] : "")
            + input[(middleIndex + SplitIdentifierHierarchy[splitIdentifierIdx].Length)..];

        List<string> output = new();

        output.AddRange(GetQualifiedSubstrings(substr1, splitIdentifierIdx));
        output.AddRange(GetQualifiedSubstrings(substr2, splitIdentifierIdx));

        return output;
    }

    private List<string> GetQualifiedSubstrings(string substr, int splitIdentifierIdx)
    {
        if (Measure(substr) <= MaxStringTokens) { return new() { substr }; }
        else { return SplitWithCurrentSplitIdentifier(substr, splitIdentifierIdx); }
    }

    private int FindClosestSubStrToMiddle(string input, int splitIdentifierIdx)
    {
        var commaIndices = GetCommaIndices(input, splitIdentifierIdx);

        if (!commaIndices.Any())
            return -1;

        int middleIndex = input.Length / 2;

        return commaIndices.OrderBy(idx => Math.Abs(idx - middleIndex)).First();
    }

    private List<int> GetCommaIndices(string input, int splitIdentifierIdx)
    {
        return input.Select((c, i) => input[i..].StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]) ? i : -1).Where(i => i >= 0).ToList();
    }

    private IEnumerable<string> CharacterSplitByTokenNum(string s)
    {
        for (var i = 0; i < s.Length; i += MaxStringTokens)
            yield return s.Substring(i, Math.Min(MaxStringTokens, s.Length - i));
    }

    protected int Measure(string input)
    {
        return Tokenizer.CountTokens(input);
    }
}
