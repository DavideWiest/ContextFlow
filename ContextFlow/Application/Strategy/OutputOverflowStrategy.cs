using ContextFlow.Application.Request;
using ContextFlow.Domain;
using System;
namespace ContextFlow.Application.Strategy;

public abstract class OutputOverflowStrategy : FailStrategy<OutputOverflowException>
{
    public abstract override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e);
}

public class OutputOverflowStrategyRestrictOutputLength : OutputOverflowStrategy
{
    private readonly int TokenToWordRatio;
    private readonly double MarginOfSafetyMul;

    public OutputOverflowStrategyRestrictOutputLength() { }
    public OutputOverflowStrategyRestrictOutputLength(int tokenToWordRatio=3, double marginOfSafetyMul=0.8)
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

public class OutputOverflowStrategyThrowException : OutputOverflowStrategy
{
    public override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e)
    {
        throw e;
    }
}

