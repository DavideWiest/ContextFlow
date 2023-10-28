using ContextFlow.Application.Request;
using ContextFlow.Application.Strategy;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Fakes;
using Tests.Sample;

namespace Tests.Strategy;

public class TestLLMFailStrategyRetrySameContext
{
    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingPrompt(SampleRequests.sampleRequest.Prompt)
                .UsingLLMConfig(SampleRequests.sampleRequest.LLMConfig);
                

    [Test]
    public void TestRetrySameCtx()
    {
        LLMRequest request = requestBuilder
            .UsingLLMConnection(new ThrowThenSayHiConnectionAfterN(2))
            .UsingRequestConfig(new RequestConfig().UsingFailStrategy(new LLMFailStrategyRetrySameSettings(2)))
            .Build();

        try
        {
            request.Complete();
            Assert.Pass();
        }
        catch (LLMException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public void TestRetrySameCtxExceedMaxRetries()
    {
        LLMRequest request = requestBuilder
            .UsingLLMConnection(new ThrowThenSayHiConnectionAfterN(3))
            .UsingRequestConfig(new RequestConfig().UsingFailStrategy(new LLMFailStrategyRetrySameSettings(2)))
            .Build();

        try
        {
            request.Complete();
            Assert.Fail();
        }
        catch (LLMException)
        {
            Assert.Pass();
        }
    }
}
