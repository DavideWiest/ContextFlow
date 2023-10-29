﻿using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI.Async;

using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

public class OpenAICompletionConnectionAsync : LLMConnectionAsync
{
    OpenAIAPI api;

    public OpenAICompletionConnectionAsync(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAICompletionConnectionAsync()
    {
        // uses default, env ("OPENAI_API_KEY"), or config file
        api = new();
    }

    protected override async Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            var result = await OpenAIUtil.GetCompletionResult(api, input, conf, log);
            return OpenAIUtil.CompletionResultToRequestResult(result);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}
