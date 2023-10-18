using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Application.TextUtil;
using Serilog;
using Serilog.Core;
using OpenAI_API.Moderation;

namespace ContextFlow.Application;

public class LLMRequest
{
    private CFLogger log = new CFDefaultLogger();
    private Prompt Prompt;
    private CFConverter outputConverter;
    private bool outputIsString = true;
    //private Type outputType = typeof(String);

    LLMConfig LLMConfig;
    LLMConnection LLMConnection;
    RequestConfig RequestConfig;

    public LLMRequest(LLMConfig llmconf, LLMConnection llmcon, RequestConfig requestconf)
    {
        LLMConfig = llmconf;
        LLMConnection = llmcon;
        RequestConfig = requestconf;
    }

    public LLMRequest UsingLogger(CFLogger log)
    {
        SetLogger(log); 
        return this;
    }

    public void SetLogger(CFLogger log) {
        this.log = log;
    }

    public LLMRequest UsingPrompt(Prompt prompt)
    {
        SetPrompt(prompt);
        return this;
    }

    public void SetPrompt(Prompt prompt)
    {
        Prompt = prompt;
    }

    public LLMRequest UsingOutputConverter(CFConverter converter)
    {
        SetOutputConverter(converter);
        return this;
    }

    public void SetOutputConverter(CFConverter converter)
    {
        outputConverter = converter;
    }
    public LLMRequest UsingOutputisString(bool isString)
    {
        SetOutputType(isString);
        return this;
    }

    public void SetOutputType(bool isString)
    {
        outputIsString = isString;
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
        if (!outputIsString)
        {
            try
            {
                CheckConverterExists();

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
        if (outputIsString || (parsedOutput == null && RequestConfig.PassAsStringIfNoConverterDefined))
        {
            parsedOutput = partialresult.RawOutput;
        }

        return new RequestResult(partialresult, parsedOutput);
    }

    private void CheckPromptExists()
    {
        if (Prompt == null)
        {
            log.Error("Cannot complete a request without a prompt. Use UsingPrompt or SetPrompt first.");
            throw new InvalidOperationException("Cannot complete a request without a prompt. Use UsingPrompt or SetPrompt first.");
        }
    }
    private void CheckConverterExists()
    {
        if (outputConverter == null)
        {
            throw new InvalidOperationException("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter or SetConverter to fix it.");
        }
    }
}
