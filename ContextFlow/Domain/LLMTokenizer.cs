using System;

namespace ContextFlow.Domain;

public abstract class LLMTokenizer
{
    public abstract int CountTokens(string input);
}
