using ContextFlow.Application.Request;
using ContextFlow.Infrastructure.Providers;

namespace ContextFlow.Application.Templates;

public static class RequestTemplates
{

    public static LLMRequest Aggregate(IEnumerable<string> texts, LLMRequestBuilder preconfiguredBuilder)
    {
        return preconfiguredBuilder
            .Build();
    }
}
