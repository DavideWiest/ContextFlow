using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Storage;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Fakes;
using System.Reflection;

namespace Tests.Sample;

public static class SampleRequests
{
    public static LLMRequest sampleRequest = new LLMRequest(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnection(), new RequestConfig());
    public static LLMRequestAsync sampleRequestAsync = new LLMRequestAsync(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnectionAsync(), new RequestConfigAsync());

    public static string sampleRequestCorrectResultFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/CorrectSampleRequestSaved.json";
}
