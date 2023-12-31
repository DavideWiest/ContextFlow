﻿using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Strategy.Async;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Fakes.Async;
using Tests.Sample;

namespace Tests.Strategy;

public class TestLLMFailStrategyRetryNewContextAsync
{
    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingPrompt(SampleRequests.sampleRequest.Prompt)
                .UsingLLMConfig(SampleRequests.sampleRequest.LLMConfig);

    [Test]
    public async Task TestRetryNewCtx()
    {
        LLMRequestAsync request = requestBuilder
            .UsingLLMConnection(new ThrowOrSayHiUnderConditionConnectionAsync((prompt, llmconf) => !prompt.Contains("Now don't fail")))
            .UsingRequestConfig(new RequestConfigAsync().AddFailStrategy(
                new FailStrategyRetryNewSettingsAsync<LLMConnectionException>(1, null, null, new Prompt("say hi").UsingAttachment(new Attachment("Info", "Now don't fail", true))))
            )
            .BuildAsync();

        try
        {
            await request.Complete();
            Assert.Pass();
        }
        catch (LLMConnectionException)
        {
            Assert.Fail();
        }
    }

    [Test]
    public async Task TestRetryNewCtxExceedMaxRetries()
    {
        LLMRequestAsync request = requestBuilder
            .UsingLLMConnection(new ThrowOrSayHiUnderConditionConnection((prompt, llmconf) => !prompt.Contains("Now don't fail")))
            .UsingRequestConfig(new RequestConfigAsync().AddFailStrategy(
                new FailStrategyRetryNewSettingsAsync<LLMConnectionException>(1, null, null, new Prompt("say hi").UsingAttachment(new Attachment("Info", "Fail", true))))
            )
            .BuildAsync();

        try
        {
            await request.Complete();
            Assert.Fail();
        }
        catch (LLMConnectionException)
        {
            Assert.Pass();
        }
    }
}
