namespace ContextFlow.Application.TextUtil;

using ContextFlow.Domain;

public class HierarchichalTextSplitter: TextSplitter
{

    private readonly List<string> SplitIdentifierHierarchy;
    private readonly List<string> IdentifiersToAddToBeginnings;
    private readonly int MaxStringTokens;
    private LLMTokenizer Tokenizer;

    public HierarchichalTextSplitter(LLMTokenizer tokenizer, int maxStringTokens, List<string> splitIdentifierHierarchy, List<string> identifiersToAddToBeginnings)
    {
        Tokenizer = tokenizer;
        if (splitIdentifierHierarchy.Count == 0)
        {
            throw new InvalidDataException("SplitIdentifierHierarchy must have at least one value");
        }
        SplitIdentifierHierarchy = splitIdentifierHierarchy;
        IdentifiersToAddToBeginnings = identifiersToAddToBeginnings;
        MaxStringTokens = maxStringTokens;
    }

    public override List<string> Split(string input)
    {
        return Split(input, 0);
    }

    public List<string> Split(string input, int splitIdentifierIdx)
    {
        string splittableInput = input.StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]) ? 
           input.Substring(SplitIdentifierHierarchy[splitIdentifierIdx].Length) : input;

        if (!splittableInput.Contains(SplitIdentifierHierarchy[splitIdentifierIdx]))
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

    protected List<string> SplitWithCurrentSplitIdentifier(string input, int splitIdentifierIdx)
    {
        if (SplitIdentifierHierarchy[splitIdentifierIdx] == "")
        {
            return CharacterSplitByTokenNum(input).ToList();
        }
        bool inputStartsWithIdentifier = input.StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]);
        string splittableInput = inputStartsWithIdentifier ?
           input.Substring(SplitIdentifierHierarchy[splitIdentifierIdx].Length) : input;

        int middleIndex = FindClosestSubStrToMiddle(splittableInput, splitIdentifierIdx);

        if (middleIndex == -1)
        {
            return new() { input };
        }

        string substr1 = (inputStartsWithIdentifier && IdentifiersToAddToBeginnings.Contains(SplitIdentifierHierarchy[splitIdentifierIdx])
            ? SplitIdentifierHierarchy[splitIdentifierIdx] : "") + input.Substring(0, middleIndex);

        string substr2 = 
            (IdentifiersToAddToBeginnings.Contains(SplitIdentifierHierarchy[splitIdentifierIdx])
            ? SplitIdentifierHierarchy[splitIdentifierIdx] : "")
            + input.Substring(middleIndex + SplitIdentifierHierarchy[splitIdentifierIdx].Length);
        List<string> output = new List<string>();

        if (Measure(substr1) <= MaxStringTokens)
        {
            output.Add(substr1);
        } else { output.AddRange(SplitWithCurrentSplitIdentifier(substr1, splitIdentifierIdx)); }

        if (Measure(substr2) <= MaxStringTokens)
        {
            output.Add(substr2);
        }
        else { output.AddRange(SplitWithCurrentSplitIdentifier(substr2, splitIdentifierIdx)); }

        return output;
    }

    protected int FindClosestSubStrToMiddle(string input, int splitIdentifierIdx)
    {
        var commaIndices = input.Select((c, i) => input.Substring(i).StartsWith(SplitIdentifierHierarchy[splitIdentifierIdx]) ? i : -1).Where(i => i >= 0).ToList();

        if (!commaIndices.Any())
            return -1;

        int middleIndex = input.Length / 2;

        return commaIndices.OrderBy(idx => Math.Abs(idx - middleIndex)).First();
    }

    protected IEnumerable<string> CharacterSplitByTokenNum(string s)
    {
        for (var i = 0; i < s.Length; i += MaxStringTokens)
            yield return s.Substring(i, Math.Min(MaxStringTokens, s.Length - i));
    }

    protected int Measure(string input)
    {
        return Tokenizer.CountTokens(input);
    }
}
