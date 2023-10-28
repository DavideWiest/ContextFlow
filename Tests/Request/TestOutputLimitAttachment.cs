using ContextFlow.Application.Request;
using ContextFlow.Domain;
using Tests.Sample;

namespace Tests.Request;

public class TestOutputLimitAttachment
{
    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingPrompt(SampleRequests.sampleRequest.Prompt)
                .UsingLLMConfig(new LLMConfig("model").UsingMaxTotalTokens(200).UsingMaxInputTokens(100))
                .UsingRequestConfig(SampleRequests.sampleRequest.RequestConfig)
                .UsingLLMConnection(SampleRequests.sampleRequest.LLMConnection);


    [Test]
    public void TestMatchExistsAndLoadMatch()
    {
        var request = requestBuilder.Build().UsingOutputLimitAttachment(3, 0.5);
        Assert.That(request.Prompt.ToPlainText().Contains($"The output must be under 16 words long"));
    }
}
