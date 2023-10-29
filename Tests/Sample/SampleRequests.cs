using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Fakes.Async;

namespace Tests.Sample;

public static class SampleRequests
{
    public static LLMRequest sampleRequest = new LLMRequest(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnection(), new RequestConfig());
    public static LLMRequestAsync sampleRequestAsync = new LLMRequestAsync(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnectionAsync(), new RequestConfigAsync());

    public static string sampleRequestCorrectResultFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/CorrectSampleRequestSaved.json";
}
