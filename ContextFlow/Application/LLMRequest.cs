using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Application.TextUtil;

namespace ContextFlow.Application;

public class LLMRequest
{
    protected CFLogger log = new CFDefaultLogger();
    protected CFConverter outputConverter = new DefaultConverter(true);

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
        log.Debug("\n--- PROMPT ---\n" + Prompt.ToPlainText() + "\n--- PROMPT ---\n");

        CheckPromptExists();

        if (RequestConfig.GetCheckNumTokensBeforeRequest())
        {
            CheckPromptTokens(RequestConfig.Tokenizer, true);
        }

        PartialRequestResult partialresult = LLMConnection.GetResponse(Prompt.ToPlainText(), LLMConfig, log);

        log.Debug("\n--- RAW OUTPUT ---\n" + partialresult.RawOutput + "\n--- RAW OUTPUT ---\n");

        dynamic? parsedOutput = null;
        if (!RequestConfig.ParseOutputToDynamic)
        {
            try
            {
                parsedOutput = outputConverter.ToDynamic(partialresult.RawOutput);
            } catch (InvalidOperationException e)
            {
                if (RequestConfig.PassAsStringIfNoConverterDefined)
                {
                    log.Error(e.Message, e);
                    log.Warning("Automatically passing it as plain string instead of converting it. This may cause issues later on. Not recommended.");
                } else
                {
                    throw new InvalidOperationException(e.Message);
                }
            }
        }

        // if the preceding if statement didnt work, or was not triggered, in which in both cases it is null
        if (RequestConfig.ParseOutputToDynamic || (parsedOutput == null && RequestConfig.PassAsStringIfNoConverterDefined))
        {
            parsedOutput = partialresult.RawOutput;
        }

        return new RequestResult(partialresult, parsedOutput);
    }

    protected void CheckPromptExists()
    {
        if (Prompt == null)
        {
            log.Error("Cannot complete a request without a prompt. Use UsingPrompt or SetPrompt first.");
            throw new InvalidOperationException("Cannot complete a request without a prompt. Use UsingPrompt or SetPrompt first.");
        }
    }
    protected void CheckConverterExists()
    {
        if (outputConverter == null)
        {
            throw new InvalidOperationException("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter or SetConverter to fix it.");
        }
    }
}
