using ContextFlow.Domain;

namespace ContextFlow.Infrastructure.Providers;

/// <summary>
/// A tokenizer interface 
/// </summary>
public abstract class LLMTokenizer
{
    public abstract int CountTokens(string input);

    /// <summary>
    /// Validates the number of tokens
    /// </summary>
    /// <param name="input"></param>
    /// <param name="tokenMax"></param>
    /// <exception cref="InputOverflowException">Throws this exception if the token-limit is exceeded</exception>
    public void ValidateNumTokens(string input, int tokenMax)
    {
        int num = CountTokens(input);
        if (num > tokenMax)
        {
            throw new InputOverflowException($"Input exceeds the given token limit [limit={tokenMax}, actual={num}]");
        }
    }

    /// <summary>
    /// Checks if the number of tokens does not exceed the maximum tokens. Be aware that you need to leave buffer for the output, as is most likely limited too.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="tokenMax"></param>
    /// <param name="throwExcIfExceeding"></param>
    /// <returns></returns>
    public bool CheckTokenNumPasses(string input, int tokenMax)
    {
        return CountTokens(input) <= tokenMax;
    }
}
