namespace Tests;

using ContextFlow.Infrastructure.Providers.OpenAI;

public class Tests
{

    private OpenAIChatConnection llmcon = new();
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestRequest()
    {
        llmcon.GetResponse("Say hi");
    }
}