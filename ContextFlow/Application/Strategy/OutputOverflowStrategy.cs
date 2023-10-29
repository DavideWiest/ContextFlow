using ContextFlow.Application.Request;
using ContextFlow.Application.Result;
using ContextFlow.Domain;
namespace ContextFlow.Application.Strategy;

public abstract class OutputOverflowStrategyAsync : FailStrategy<OutputOverflowException>
{
    public abstract override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e);
}

public class OutputOverflowStrategyRestrictOutputLength : OutputOverflowStrategyAsync
{
    private readonly int TokenToWordRatio;
    private readonly double MarginOfSafetyMul;

    public OutputOverflowStrategyRestrictOutputLength() { }
    public OutputOverflowStrategyRestrictOutputLength(int tokenToWordRatio = 4, double marginOfSafetyMul = 0.75)
    {
        TokenToWordRatio = tokenToWordRatio;
        MarginOfSafetyMul = marginOfSafetyMul;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e)
    {
        request.UsingOutputLimitAttachment(TokenToWordRatio, MarginOfSafetyMul);
        request = new LLMRequestBuilder(request)
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(new FailStrategyThrowException<OutputOverflowException>()))
            .Build();
        return request.Complete();
    }
}

public class OutputOverflowStrategyThrowException : OutputOverflowStrategyAsync
{
    public override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e)
    {
        throw e;
    }
}

