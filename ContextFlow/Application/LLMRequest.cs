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
    protected CFConverter outputConverter = new DynamicConverterOld(true);

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
        return tokenizer.CheckTokenNumPasses(Prompt.ToPlainText(), LLMConfig.MaxTokensInput, throwExcIfExceeding);
    }

    public RequestResult Complete()
    {
        RequestConfig.log.Debug("\n--- PROMPT ---\n" + Prompt.ToPlainText() + "\n--- PROMPT ---\n");

        CheckPromptExists();

        if (RequestConfig.GetCheckNumTokensBeforeRequest())
        {
            CheckPromptTokens(RequestConfig.Tokenizer!, true);
        }

        RequestResult result = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, log);

        RequestConfig.log.Debug("\n--- RAW OUTPUT ---\n" + result.RawOutput + "\n--- RAW OUTPUT ---\n");

        return result;
    }

    protected void CheckPromptExists()
    {
        if (Prompt == null)
        {
            RequestConfig.log.Error("Cannot complete a request without a prompt. Use UsingPrompt first.");
            throw new InvalidOperationException("Cannot complete a request without a prompt. Use UsingPrompt first.");
        }
    }
    protected void CheckConverterExists()
    {
        if (outputConverter == null)
        {
            RequestConfig.log.Error("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter to fix it.");
            throw new InvalidOperationException("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter to fix it.");
        }
    }
}
