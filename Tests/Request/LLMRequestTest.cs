
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI;
using Tests.Fakes;

namespace Tests.Request;

public class LLMRequestTest
{
    LLMConnection llmcon = new OpenAIChatConnection();
    LLMConnection llmconHi = new SayHiConnection();
    LLMConfig llmconfCompletions = new("gpt-3.5-turbo-16k");
    RequestConfig requestconf = new();


    [SetUp]
    public void Setup()
    {
        llmconfCompletions.UsingMaxInputTokens(50);
        llmconfCompletions.UsingMaxTotalTokens(100);
    }

    [Test]
    public void TestHi()
    {
        RequestResult res = new LLMRequest(new Prompt("Say \"Hi\"."), llmconfCompletions, llmconHi, requestconf).Complete();
        Assert.That(res.RawOutput.ToLower().StartsWith("hi"));
    }

    [Test]
    public void TestPromptPipeline()
    {
        string input = "Hi Hi Hi";
        Prompt prompt = new Prompt("Say \"Hi\" 1 time less than the given input. Respond only with the \"Hi\"s as shown and nothing else.");

        foreach (int i in Enumerable.Range(0, 2))
        {
            input = new LLMRequest(prompt.UpsertingAttachment(new Attachment("Input", input, false)), llmconfCompletions, llmcon, requestconf).Complete().RawOutput;
        }

        Assert.That(input, Is.EqualTo("Hi"));
    }

    [Test]
    public void TestThrowExceptionOnOutputOverflow()
    {
        try
        {
            new LLMRequest(new Prompt("test"), llmconfCompletions, new OutputOverFlowConnection(), new RequestConfig().UsingRaiseExceptionOnOutputOverflow(true)).Complete();
            Assert.Fail();
        }
        catch (OutputOverflowException)
        {
            Assert.Pass();
        }
    }
}
