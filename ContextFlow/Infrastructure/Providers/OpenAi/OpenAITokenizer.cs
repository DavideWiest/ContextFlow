using Microsoft.DeepDev;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;

public class OpenAITokenizer : LLMTokenizer
{
    private static string IM_START = "<|im_start|>";
    private static string IM_END = "<|im_end|>";

    private ITokenizer tokenizer;

    private Dictionary<string, int> specialTokens = new Dictionary<string, int> {
        {IM_START, 100264},
        {IM_END, 100265},
    };

    public OpenAITokenizer(string modelName)
    {
        tokenizer = TokenizerBuilder.CreateByModelNameAsync(modelName, specialTokens).GetAwaiter().GetResult();
    }

    public override int CountTokens(string input)
    {
        var encoded = tokenizer.Encode(IM_START + input + IM_END, new HashSet<string>(specialTokens.Keys));
        return encoded.Count;
    }
}
