using ContextFlow.Infrastructure.Providers;

namespace Tests.Fakes;

public class CountCharsTokenizer : LLMTokenizer
{
    public override int CountTokens(string input)
    {
        return input.Length;
    }
}
