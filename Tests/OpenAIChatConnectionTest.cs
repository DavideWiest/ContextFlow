namespace Tests;

using ContextFlow.Infrastructure.Providers.OpenAI;

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
        // llmcon.GetResponse("Say hi");
        Assert.Pass();
    }
}