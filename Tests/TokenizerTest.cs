using ContextFlow.Infrastructure.Providers.OpenAI;

namespace Tests;

public class Tests
{


    private string tokenTestString ="""
            Many words map to one token, but some don't: indivisible.

            Unicode characters like emojis may be split into many tokens containing the underlying bytes: 🤚🏾

            Sequences of characters commonly found next to each other may be grouped together: 1234567890
            """;
    int expectedTokens = 59;

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void CountTokens_Gpt3_5_Turbo()
    {
        int ntokens = new OpenAITokenizer("gpt-3.5-turbo-16k-0613").CountTokens(tokenTestString);
        Assert.AreEqual(expectedTokens, ntokens);
    }
    [Test]
    public void CountTokens_2_Gpt4()
    {
        int ntokens = new OpenAITokenizer("gpt-4").CountTokens(tokenTestString);
        Assert.AreEqual(expectedTokens, ntokens);
    }

}