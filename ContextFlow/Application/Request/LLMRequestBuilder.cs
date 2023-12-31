﻿using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Request;

public class LLMRequestBuilder
{
    protected Prompt? Prompt;
    protected LLMConfig? LLMConfig;
    protected LLMConnection? LLMConnection;
    protected LLMConnectionAsync? LLMConnectionAsync;
    protected RequestConfig? RequestConfig;
    protected RequestConfigAsync? RequestConfigAsync;

    public LLMRequestBuilder() { }

    public LLMRequestBuilder(LLMRequest request) : this(request.Prompt, request.LLMConfig)
    {
        LLMConnection = request.LLMConnection;
        RequestConfig = request.RequestConfig;
    }

    public LLMRequestBuilder(LLMRequestAsync request) : this(request.Prompt, request.LLMConfig)
    {
        LLMConnectionAsync = request.LLMConnection;
        RequestConfigAsync = request.RequestConfig;
    }

    private LLMRequestBuilder(Prompt prompt, LLMConfig llmConfig)
    {
        Prompt = prompt;
        LLMConfig = llmConfig;
    }

    public LLMRequestBuilder UsingPrompt(Prompt prompt)
    {
        Prompt = prompt;
        return this;
    }

    public LLMRequestBuilder UsingLLMConfig(LLMConfig config)
    {
        LLMConfig = config;
        return this;
    }

    public LLMRequestBuilder UsingLLMConnection(LLMConnection connection)
    {
        LLMConnection = connection;
        return this;
    }

    public LLMRequestBuilder UsingLLMConnection(LLMConnectionAsync connection)
    {
        LLMConnectionAsync = connection;
        return this;
    }

    public LLMRequestBuilder UsingRequestConfig(RequestConfig config)
    {
        RequestConfig = config;
        return this;
    }

    public LLMRequestBuilder UsingRequestConfig(RequestConfigAsync config)
    {
        RequestConfigAsync = config;
        return this;
    }

    /// <summary>
    /// Will return the built LLMRequest after validating that the required parameters have been set.
    /// </summary>
    /// <returns></returns>
    public LLMRequest Build()
    {
        Validate(false);

        return new LLMRequest(Prompt!, LLMConfig!, LLMConnection!, RequestConfig!);
    }
    /// <summary>
    /// Will return the built LLMRequestAsync after validating that the required parameters have been set.
    /// </summary>
    /// <returns></returns>
    public LLMRequestAsync BuildAsync()
    {
        Validate(true);

        return new LLMRequestAsync(Prompt!, LLMConfig!, LLMConnectionAsync!, RequestConfigAsync!);
    }

    protected void Validate(bool forAsyncClass = false)
    {
        if (!ConfigurationIsComplete(out IEnumerable<string> missingElements, forAsyncClass))
        {
            throw new InvalidOperationException("Cannot build LLMRequest yet. Missing elements to configure (Use Using... Methods: " + string.Join(", ", missingElements));
        }
    }

    /// <summary>
    /// Checks if all the necessary parameters have been set
    /// </summary>
    /// <param name="missingElements"></param>
    /// <param name="forAsyncClass">If the request class is supposed to be async or not</param>
    /// <returns></returns>
    public bool ConfigurationIsComplete(out IEnumerable<string> missingElements, bool forAsyncClass)
    {
        var missingElementsDict = GetElementDefinitionDict(forAsyncClass);

        missingElements = missingElementsDict.Keys.Where(k => missingElementsDict[k]);
        return missingElements.ToList().Count == 0;
    }

    private Dictionary<string, bool> GetElementDefinitionDict(bool forAsyncClass)
    {
        var missingElementsDict = new Dictionary<string, bool>
        {
            ["Prompt"] = Prompt == null,
            ["LLMConfig"] = LLMConfig == null,
            ["LLMConnection"] = (forAsyncClass ? LLMConnectionAsync == null : LLMConnection == null),
            ["RequestConfig"] = (forAsyncClass ? RequestConfigAsync == null : RequestConfig == null)
        };
        return missingElementsDict;
    }
}
