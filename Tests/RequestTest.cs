
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI;
using NUnit.Framework.Constraints;

namespace Tests;

public class RequestTest
{
    LLMConnection llmcon = new OpenAIChatConnection();
    LLMConfig llmconf = new("gpt-3.5-turbo-16k");
    RequestConfig requestconf = new();
    

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestDoublePrompt()
    {
        RequestResult res = new LLMRequest(new Prompt("Say \"Hi\"."), llmconf, llmcon, requestconf).Complete();
        Assert.That(res.RawOutput.StartsWith("Hi"));
    }
}
