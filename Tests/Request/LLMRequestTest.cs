
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI;
using ContextFlow.Infrastructure.Providers.OpenAI.Async;
using Tests.Fakes;

namespace Tests.Request;

public class LLMRequestTest
{
    LLMConnection llmcon = new OpenAICompletionConnection();
    LLMConnection llmconHi = new SayHiConnection();
    LLMConnectionAsync llmconAsync = new OpenAICompletionConnectionAsync();
    LLMConnectionAsync llmconHiAsync = new SayHiConnectionAsync();
    LLMConfig llmconf = new("gpt-3.5-turbo-16k");
    RequestConfig requestconf = new();
    RequestConfigAsync requestconfAsync = new();


    [SetUp]
    public void Setup()
    {
        llmconf.UsingMaxInputTokens(50);
        llmconf.UsingMaxTotalTokens(100);
    }

    [Test]
    public void TestHi()
    {
        RequestResult res = new LLMRequest(new Prompt("Say \"Hi\"."), llmconf, llmconHi, requestconf).Complete();
        Assert.That(res.RawOutput.StartsWith("Hi"));
    }

    [Test]
    public async Task TestAsyncHi()
    {
        RequestResultAsync res = await new LLMRequestAsync(new Prompt("Say \"Hi\"."), llmconf, llmconHiAsync, requestconfAsync).CompleteAsync();
        Assert.That(res.RawOutput.StartsWith("Hi"));
    }

    [Test]
    public void TestPromptPipeline()
    {
        string input = "Hi Hi Hi";
        Prompt prompt = new Prompt("Say \"Hi\" 1 time less than the given input. Respond only with the \"Hi\"s as shown and nothing else.");

        foreach (int i in Enumerable.Range(0, 2))
        {
            input = new LLMRequest(prompt.UsingAttachment("Input", input), llmconf, llmcon, requestconf).Complete().RawOutput;
        }

        Assert.That(input, Is.EqualTo("Hi"));
    }
}
