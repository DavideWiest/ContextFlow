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
    protected CFLogger log = new CFDefaultLogger();
    protected CFConverter outputConverter = new DefaultConverter(true);

    protected Prompt? Prompt;
    protected LLMConfig? LLMConfig;
    protected LLMConnection? LLMConnection;
    protected RequestConfig? RequestConfig;

    public LLMRequestBuilder UsingLogger(CFLogger log)
    {
        SetLogger(log);
        return this;
    }

    public void SetLogger(CFLogger log)
    {
        this.log = log;
    }

    public LLMRequestBuilder UsingOutputConverter(CFConverter converter)
    {
        SetOutputConverter(converter);
        return this;
    }

    public void SetOutputConverter(CFConverter converter)
    {
        outputConverter = converter;
    }



    public LLMRequestBuilder UsingPrompt(Prompt prompt)
    {
        SetPrompt(prompt);
        return this;
    }

    public void SetPrompt(Prompt prompt)
    {
        Prompt = prompt;
    }

    public LLMRequestBuilder SetLLMConfig(LLMConfig config)
    {
        this.LLMConfig = config;
        return this;
    }

    public LLMRequestBuilder UsingLLMConfig(LLMConfig config)
    {
        SetLLMConfig(config);
        return this;
    }

    public LLMRequestBuilder SetLLMConnection(LLMConnection connection)
    {
        this.LLMConnection = connection;
        return this;
    }

    public LLMRequestBuilder UsingLLMConnection(LLMConnection connection)
    {
        SetLLMConnection(connection);
        return this;
    }

    public LLMRequestBuilder SetRequestConfig(RequestConfig config)
    {
        this.RequestConfig = config;
        return this;
    }

    public LLMRequestBuilder UsingRequestConfig(RequestConfig config)
    {
        SetRequestConfig(config);
        return this;
    }

    public LLMRequest Build()
    {
        List<string> missingElements = new();
        if (!ConfigurationIsComplete(out missingElements))
        {
            throw new InvalidOperationException("Cannot build logger yet. Missing elements to configure (Use Set... or Using... Methods: " + String.Join(", ", missingElements);
        }

        var request = new LLMRequest(Prompt, LLMConfig, LLMConnection, RequestConfig);
        

        return request;
    }

    protected bool ConfigurationIsComplete(out List<string> missingElements)
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
