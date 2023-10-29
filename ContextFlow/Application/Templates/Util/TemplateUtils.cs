using ContextFlow.Application.Request;
using ContextFlow.Domain;

namespace ContextFlow.Application.Templates.Util;

internal static class TemplateUtils
{
    public static RequestConfig SentitiveRequestConfig = new RequestConfig().DeactivateCheckNumTokensBeforeRequest();

    public static LLMConfig GetDefaultLLMConfig(string modelName, int maxTotalTokens)
    {
        return new LLMConfig(modelName, maxTotalTokens);
    }
}
