﻿using ContextFlow.Application.Request;
using ContextFlow.Application.Strategy;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Sample;

namespace Tests.Strategy;

public class TestLLMFailStrategyRetrySameContextAsync
{
    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingPrompt(SampleRequests.sampleRequest.Prompt)
                .UsingLLMConfig(SampleRequests.sampleRequest.LLMConfig);


    [Test]
    public void TestRetrySameCtx()
    {
        LLMRequest request = requestBuilder
            .UsingLLMConnection(new ThrowThenSayHiConnectionAfterN(2))
            .UsingRequestConfig(new RequestConfig().AddFailStrategy(new FailStrategyRetrySameSettings<LLMException>(2)))
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
            .UsingLLMConnection(new ThrowThenSayHiConnectionAfterN(4))
            .UsingRequestConfig(new RequestConfig().AddFailStrategy(new FailStrategyRetrySameSettings<LLMException>(2)))
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
