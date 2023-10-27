
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI;
using ContextFlow.Infrastructure.Providers.OpenAI.Async;

namespace Tests;

public class RequestTest
{
    LLMConnection llmcon = new OpenAIChatConnection();
    LLMConnectionAsync llmconAsync = new OpenAIChatConnectionAsync();
    LLMConfig llmconf = new("gpt-3.5-turbo-16k");
    RequestConfig requestconf = new();
    RequestConfigAsync requestconfAsync = new();


    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void TestHi()
    {
        RequestResult res = new LLMRequest(new Prompt("Say \"Hi\"."), llmconf, llmcon, requestconf).Complete();
        Assert.That(res.RawOutput.StartsWith("Hi"));
    }

    [Test]
    public void TestAsyncHi()
    {
        RequestResultAsync res = new LLMRequestAsync(new Prompt("Say \"Hi\"."), llmconf, llmcon, requestconf).CompleteAsync();
        Assert.That(res.RawOutput.StartsWith("Hi"));
    }
}
