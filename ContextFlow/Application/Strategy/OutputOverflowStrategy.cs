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
    private readonly int TokenToWordRatio = 3;
    private readonly double MarginOfSafetyMul = 0.8;

    public OutputOverflowStrategyRestrictOutputLength() { }
    public OutputOverflowStrategyRestrictOutputLength(int tokenToWordRatio, double marginOfSafetyMul)
    {
        TokenToWordRatio = tokenToWordRatio;
        MarginOfSafetyMul = marginOfSafetyMul;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, OutputOverflowException e)
    {
        request.UsingOutputLimitAttachment(TokenToWordRatio, MarginOfSafetyMul);
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

