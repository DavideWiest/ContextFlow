﻿using OpenAI_API;

namespace ContextFlow.Infrastructure.Providers.OpenAI;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers;

public class OpenAICompletionConnection : LLMConnectionAsync
{
    OpenAIAPI api;

    public OpenAICompletionConnection(string apiKey)
    {
        api = new(apiKey);
    }

    public OpenAICompletionConnection()
    {
        // uses default, env ("OPENAI_API_KEY"), or config file
        api = new();
    }

    protected override async Task<RequestResult> CallAPIAsync(string input, LLMConfig conf, CFLogger log)
    {
        try
        {
            var result = await OpenAIUtil.GetCompletionResult(api, input, conf, log);
            string output = result.Completions[0].ToString();
            FinishReason finish = OpenAIUtil.ToCFFinishReason(result.Completions[0].FinishReason);

            return new RequestResult(output, FinishReason.Stop);
        }
        catch (Exception e)
        {
            log.Error($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
            throw new LLMException($"Failed to get the output from the LLM. Exception: {e.GetType()}: {e.Message}");
        }
    }
}