using ContextFlow.Application.Request;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Sample;

namespace Tests.Strategy;

public class TestLLMFailStrategyThrowExceptionAsync
{
    LLMRequest request = new LLMRequest(
                SampleRequests.sampleRequest.Prompt,
                SampleRequests.sampleRequest.LLMConfig,
                new AlwaysThrowLLMExceptionConnection(),
                SampleRequests.sampleRequest.RequestConfig);

    [Test]
    public void TestThrowException()
    {
        try
        {
            request.Complete();
            Assert.Fail();
        }
        catch (LLMConnectionException)
        {
            Assert.Pass();
        }
    }
}
