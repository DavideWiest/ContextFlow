using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Application.TextUtil;
using OpenAI_API.Moderation;

namespace ContextFlow.Application;

public class LLMRequest
{
    protected CFLogger log = new CFDefaultLogger();

    protected Prompt Prompt;
    protected LLMConfig LLMConfig;
    protected LLMConnection LLMConnection;
    protected RequestConfig RequestConfig;
    
    public LLMRequest(Prompt prompt, LLMConfig llmconf, LLMConnection llmcon, RequestConfig requestconf)
    {
        Prompt = prompt;
        LLMConfig = llmconf;
        LLMConnection = llmcon;
        RequestConfig = requestconf;
    }


    public bool CheckPromptTokens(LLMTokenizer tokenizer, bool throwExcIfExceeding)
    {
        return tokenizer.CheckTokenNumPasses(Prompt.ToPlainText(), LLMConfig.MaxInputTokens, throwExcIfExceeding);
    }

    public RequestResult Complete()
    {
        RequestConfig.Logger.Debug("\n--- PROMPT ---\n" + Prompt.ToPlainText() + "\n--- PROMPT ---\n");

        if (RequestConfig.CheckNumTokensBeforeRequest)
        {
            CheckPromptTokens(RequestConfig.Tokenizer!, true);
        }

        RequestResult result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, log);

        RequestConfig.Logger.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }
}
