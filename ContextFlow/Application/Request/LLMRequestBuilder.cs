using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;

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

    public LLMRequest Build()
    {
        Validate(false);

        return new LLMRequest(Prompt!, LLMConfig!, LLMConnection!, RequestConfig!);
    }

    public LLMRequestAsync BuildAsync()
    {
        Validate(true);

        return new LLMRequestAsync(Prompt!, LLMConfig!, LLMConnectionAsync!, RequestConfigAsync!);
    }

    protected void Validate(bool forAsyncClass = false)
    {
        IEnumerable<string> missingElements;
        if (!ConfigurationIsComplete(out missingElements, forAsyncClass))
        {
            throw new InvalidOperationException("Cannot build LLMRequest yet. Missing elements to configure (Use Using... Methods: " + string.Join(", ", missingElements));
        }
    }

    public bool ConfigurationIsComplete(out IEnumerable<string> missingElements, bool forAsyncClass)
    {
        var missingElementsDict = new Dictionary<string, bool>();
        missingElementsDict["Prompt"] = Prompt == null;
        missingElementsDict["LLMConfig"] = LLMConfig == null;
        missingElementsDict["LLMConnection"] = (forAsyncClass ? LLMConnectionAsync == null : LLMConnection == null);
        missingElementsDict["RequestConfig"] = (forAsyncClass ? RequestConfigAsync == null : RequestConfig == null);

        missingElements = missingElementsDict.Keys.Where(k => missingElementsDict[k]);
        return missingElements.ToList().Count == 0;
    }
}
