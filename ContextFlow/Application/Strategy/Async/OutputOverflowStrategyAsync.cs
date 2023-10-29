using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
namespace ContextFlow.Application.Strategy.Async;

public class OutputOverflowStrategyRestrictOutputLengthAsync : FailStrategyAsync<OutputOverflowException>
{
    private readonly int TokenToWordRatio;
    private readonly double MarginOfSafetyMul;

    public OutputOverflowStrategyRestrictOutputLengthAsync() { }
    public OutputOverflowStrategyRestrictOutputLengthAsync(int tokenToWordRatio = 4, double marginOfSafetyMul = 0.75)
    {
        TokenToWordRatio = tokenToWordRatio;
        MarginOfSafetyMul = marginOfSafetyMul;
    }

    public override async Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, OutputOverflowException e)
    {
        request.UsingOutputLimitAttachment(TokenToWordRatio, MarginOfSafetyMul);
        request = new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(new FailStrategyThrowExceptionAsync<OutputOverflowException>()))
            .BuildAsync();
        return await request.Complete();
    }
}
