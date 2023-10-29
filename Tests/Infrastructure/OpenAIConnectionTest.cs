namespace Tests.Infrastructure;

using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;
using ContextFlow.Infrastructure.Providers.OpenAI;

public class OpenAIConnectionTest
{

    private OpenAIChatConnection llmconChat = new();


    [Test]
    public void TestChatRequest()
    {
        string output = llmconChat.GetResponse("say \"Hi\"", new LLMConfig("gpt-3.5-turbo-16k"), new CFSerilogLogger()).RawOutput;
        Assert.That(output.StartsWith("Hi"));
    }

    [Test]
    public void TestCompleteRequest()
    {
        string output = llmconChat.GetResponse("say \"Hi\"", new LLMConfig("gpt-3.5-turbo-16k"), new CFSerilogLogger()).RawOutput;
        Assert.That(output.StartsWith("Hi"));
    }
}