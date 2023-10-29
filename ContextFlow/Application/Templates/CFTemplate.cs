using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Templates.Util;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Templates;

public abstract class CFTemplate
{
    public abstract string Action { get; }

    public LLMRequest GetLLMRequest(LLMConnection con, string modelName, int maxTotalTokens = 1024)
    {
        return new LLMRequestBuilder()
            .UsingPrompt(GetPrompt())
            .UsingLLMConfig(GetLLMConfig(modelName, maxTotalTokens))
            .UsingLLMConnection(con)
            .UsingRequestConfig(new RequestConfig())
            .Build();
    }

    public LLMRequestAsync GetLLMRequestAsync(LLMConnectionAsync con, string modelName, int maxTotalTokens = 1024)
    {
        return new LLMRequestBuilder()
            .UsingPrompt(GetPrompt())
            .UsingLLMConfig(GetLLMConfig(modelName, maxTotalTokens))
            .UsingLLMConnection(con)
            .UsingRequestConfig(new RequestConfigAsync())
            .BuildAsync();
    }

    public virtual Prompt GetPrompt()
    {
        return ConfigurePrompt(new Prompt(Action));
    }

    protected abstract Prompt ConfigurePrompt(Prompt prompt);

    public virtual LLMConfig GetLLMConfig(string modelName, int maxTotalTokens)
    {
        return TemplateUtils.GetDefaultLLMConfig(modelName, maxTotalTokens);
    }

}