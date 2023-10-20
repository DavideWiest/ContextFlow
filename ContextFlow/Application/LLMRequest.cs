using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Application.TextUtil;
using OpenAI_API.Moderation;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application;

public class LLMRequest
{
    protected CFLogger log = new CFSerilogLogger();

    public Prompt Prompt { get; }
    public LLMConfig LLMConfig {get; }
    public LLMConnection LLMConnection {get; }
    public RequestConfig RequestConfig {get; }
    
    public LLMRequest(Prompt prompt, LLMConfig llmconf, LLMConnection llmcon, RequestConfig requestconf)
    {
        Prompt = prompt;
        LLMConfig = llmconf;
        LLMConnection = llmcon;
        RequestConfig = requestconf;
    }

    public RequestResult Complete()
    {
        RequestConfig.Logger.Debug("\n--- PROMPT ---\n" + Prompt.ToPlainText() + "\n--- PROMPT ---\n");

        if (RequestConfig.ValidateNumInputTokensBeforeRequest)
        {
            RequestConfig.Tokenizer!.ValidateNumTokens(Prompt.ToPlainText(), LLMConfig.MaxInputTokens);
        }

        RequestResult result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, log);

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }
}
