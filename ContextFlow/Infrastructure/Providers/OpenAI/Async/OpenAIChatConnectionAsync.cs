﻿using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI.Async;

using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

public class OpenAIChatConnectionAsync : LLMConnectionAsync
{
    OpenAIAPI api;

    public OpenAIChatConnectionAsync(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAIChatConnectionAsync()
    {
        // tries to use the environment variable OPENAI_API_KEY
        api = new();
    }

    protected override async Task<RequestResultAsync> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            var result = await OpenAIUtil.GetChatResult(api, input, conf, log);
            return OpenAIUtil.ChatResultToRequestResultAsync(result);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType().Name}: {e.Message}");
        }
    }
}