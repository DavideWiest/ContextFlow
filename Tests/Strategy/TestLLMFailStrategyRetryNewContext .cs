using ContextFlow.Application.Request;
using ContextFlow.Application.Strategy;
using ContextFlow.Domain;
using ContextFlow.Application.Prompting;
using Tests.Fakes;
using Tests.Sample;

namespace Tests.Strategy;

public class TestLLMFailStrategyRetryNewContext
{
    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingPrompt(SampleRequests.sampleRequest.Prompt)
                .UsingLLMConfig(SampleRequests.sampleRequest.LLMConfig);
                
    
    [Test]
    public void TestRetryNewCtx()
    {
        LLMRequest request = requestBuilder
            .UsingLLMConnection(new ThrowOrSayHiUnderConditionConnection((prompt, llmconf) => !prompt.Contains("Now don't fail")))
            .UsingRequestConfig(new RequestConfig().AddFailStrategy(
                new FailStrategyRetryNewSettings<LLMException>(1, null, null, new Prompt("say hi").UsingAttachmentInline("Info", "Now don't fail")))
            )
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
    public void TestRetryNewCtxExceedMaxRetries()
    {
        LLMRequest request = requestBuilder
            .UsingLLMConnection(new ThrowOrSayHiUnderConditionConnection((prompt, llmconf) => !prompt.Contains("Now don't fail")))
            .UsingRequestConfig(new RequestConfig().AddFailStrategy(
                new FailStrategyRetryNewSettings<LLMException>(1, null, null, new Prompt("say hi").UsingAttachmentInline("Info", "Fail")))
            )
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
