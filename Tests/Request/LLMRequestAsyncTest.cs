
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI.Async;
using Tests.Fakes.Async;

namespace Tests.Request;

public class LLMRequestAsyncTest
{
    LLMConnectionAsync llmconAsync = new OpenAIChatConnectionAsync();
    LLMConnectionAsync llmconHiAsync = new SayHiConnectionAsync();
    LLMConfig llmconfCompletions = new("gpt-3.5-turbo-16k");
    RequestConfigAsync requestconfAsync = new();

    [SetUp]
    public void Setup()
    {
        llmconfCompletions.UsingMaxInputTokens(50);
        llmconfCompletions.UsingMaxTotalTokens(100);
    }

    [Test]
    public async Task TestAsyncHi()
    {
        RequestResult res = await new LLMRequestAsync(new Prompt("Say \"Hi\"."), llmconfCompletions, llmconHiAsync, requestconfAsync).Complete();
        Assert.That(res.RawOutput.ToLower().StartsWith("hi"));
    }

    [Test]
    public async Task TestPromptPipelineAsync()
    {
        string input = "Hi Hi Hi";
        Prompt prompt = new Prompt("Say \"Hi\" 1 time less than the given input. Respond only with the \"Hi\"s as shown and nothing else.");

        foreach (int i in Enumerable.Range(0, 2))
        {
            input = (await new LLMRequestAsync(prompt.UpsertingAttachment(new Attachment("Input", input, false)), llmconfCompletions, llmconAsync, requestconfAsync).Complete()).RawOutput;
        }

        Assert.That(input, Is.EqualTo("Hi"));
    }

    [Test]
    public async Task TestThrowExceptionOnOutputOverflowAsync()
    {
        try
        {
            await new LLMRequestAsync(new Prompt("test"), llmconfCompletions, new OutputOverFlowConnectionAsync(), new RequestConfigAsync().UsingRaiseExceptionOnOutputOverflow(true)).Complete();
            Assert.Fail();
        } catch (OutputOverflowException)
        {
            Assert.Pass();
        }
    }
}
