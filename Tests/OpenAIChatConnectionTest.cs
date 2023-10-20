namespace Tests;

using ContextFlow.Infrastructure.Providers.OpenAI;
using ContextFlow.Domain;

public class OpenAIChatConnectionTest
{

    private OpenAIChatConnection llmcon = new();
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void TestRequest()
    {
        llmcon.GetResponse("say hi", new LLMConfig("gpt-3.5-turbo-16k", 100));
        Assert.IsTrue();
    }
}