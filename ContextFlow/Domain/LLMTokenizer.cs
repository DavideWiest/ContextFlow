using System;

namespace ContextFlow.Domain;

public abstract class LLMTokenizer
{
    public abstract int CountTokens(string input);

    /// <summary>
    /// Checks if the number of tokens does not exceed the maximum tokens. Be aware that you need to leave buffer for the output, as they will most likely also count.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="tokenMax"></param>
    /// <param name="throwExcIfExceeding"></param>
    /// <returns></returns>
    public bool CheckTokenNumPasses(string input, int tokenMax, bool throwExcIfExceeding)
    {
        bool isok = CountTokens(input) <= tokenMax;
        if (throwExcIfExceeding && isok)
        {
            throw new InvalidDataException("Input excees the given token limit");
        }
        return isok;
    }
}
