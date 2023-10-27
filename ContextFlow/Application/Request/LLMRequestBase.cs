
using ContextFlow.Application.Prompting;
using ContextFlow.Domain;

namespace ContextFlow.Application.Request;

public abstract class LLMRequestBase
{
    public Prompt Prompt { get; }
    public LLMConfig LLMConfig { get; }

    public LLMRequestBase(Prompt prompt, LLMConfig llmConfig)
    {
        Prompt = prompt;
        LLMConfig = llmConfig;
    }

    public Attachment OutputLimitAttachment(int maxInputTokens, int maxTotalTokens, double tokenToWordRatio, double marginOfSafetyMul) {
        if (tokenToWordRatio < 0) { throw new InvalidDataException("tokenToWordRatio must be positive");  }
        if (marginOfSafetyMul < 0 || marginOfSafetyMul > 1) { throw new InvalidDataException("marginOfSafetyMul must be in the range of 0 and 1."); }

        int availableTokenSpace = maxTotalTokens - maxInputTokens;
        int availableWords = (int)Math.Round(availableTokenSpace / tokenToWordRatio * marginOfSafetyMul);
        var attachment = new Attachment("Output length", $"The output must be under {availableWords} words long", true);
        return attachment;
    }
}
