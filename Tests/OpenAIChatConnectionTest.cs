namespace Tests;

using ContextFlow.Infrastructure.Providers.OpenAI;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Logging;

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
        string output = llmcon.GetResponse("say \"Hi\"", new LLMConfig("gpt-3.5-turbo-16k"), new CFSerilogLogger()).RawOutput;
        Assert.AreEqual("Hi", output);
    }
}