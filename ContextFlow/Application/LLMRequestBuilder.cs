using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application;

public class LLMRequestBuilder
{

    protected Prompt? Prompt;
    protected LLMConfig? LLMConfig;
    protected LLMConnection? LLMConnection;
    protected RequestConfig? RequestConfig;

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

    public LLMRequestBuilder UsingRequestConfig(RequestConfig config)
    {
        RequestConfig = config;
        return this;
    }

    public LLMRequest Build()
    {
        Validate();

        return new LLMRequest(Prompt!, LLMConfig!, LLMConnection!, RequestConfig!);
    }

    protected void Validate()
    {
        List<string> missingElements = new();
        if (!ConfigurationIsComplete(out missingElements))
        {
            throw new InvalidOperationException("Cannot build LLMRequest yet. Missing elements to configure (Use Using... Methods: " + String.Join(", ", missingElements));
        }
    }

    public bool ConfigurationIsComplete(out List<string> missingElements)
    {
        var missingElementsDict = new Dictionary<string, bool>();
        missingElementsDict["Prompt"] = Prompt == null;
        missingElementsDict["LLMConfig"] = LLMConfig == null;
        missingElementsDict["LLMConnection"] = LLMConnection == null;
        missingElementsDict["RequestConfig"] = RequestConfig == null;

        missingElements = missingElementsDict.Keys.Where(k => missingElementsDict[k]).ToList();
        return missingElements.Count == 0;
    }
}
