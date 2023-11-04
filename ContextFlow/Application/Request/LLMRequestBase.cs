
using ContextFlow.Application.Prompting;
using ContextFlow.Domain;

namespace ContextFlow.Application.Request;

public abstract class LLMRequestBase
{
    public Prompt Prompt { get; }
    public LLMConfig LLMConfig { get; }

    internal LLMRequestBase(Prompt prompt, LLMConfig llmConfig)
    {
        Prompt = prompt;
        LLMConfig = llmConfig;
    }

    /// <summary>
    /// Adds an attachment "Output length" that dictates the LLM that the output has to be below a calculated number of words.
    /// </summary>
    /// <param name="tokenToWordRatio">The ratio between words and tokens. 4 is a rough mean estimate, but it varies across languages.</param>
    /// <param name="marginOfSafetyMul">This will be multiplied to the word-count. Set it higher if the LLM has a higher chance of producing more tokens than it should.</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    public LLMRequestBase UsingOutputLimitAttachment(double tokenToWordRatio = 4, double marginOfSafetyMul = 0.8)
    {
        if (tokenToWordRatio < 0) { throw new InvalidDataException("tokenToWordRatio must be positive"); }
        if (marginOfSafetyMul < 0 || marginOfSafetyMul > 1) { throw new InvalidDataException("marginOfSafetyMul must be in the range of 0 and 1."); }

        int availableTokenSpace = LLMConfig.MaxTotalTokens - LLMConfig.MaxInputTokens;
        int availableWords = (int)Math.Floor(availableTokenSpace / tokenToWordRatio * marginOfSafetyMul);
        Prompt.UsingAttachment(new Attachment("Output length", $"The output must be below {availableWords} words long", true));
        return this;
    }
}
