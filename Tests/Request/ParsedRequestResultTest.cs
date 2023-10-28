using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using Tests.Fakes;
using Tests.Util;
using Tests.Sample;

namespace Tests.Request;


public class ParsedRequestResultTest
{
    ParsedRequestResult<StringWrapper> result;
    ParsedRequestResultAsync<StringWrapper> resultAsync;

    string[] input = new string[] { "yes", "no", "yes", "no", "yes" };

    [SetUp]
    public async Task Setup()
    {
        result = SampleRequests.sampleRequest.Complete().Parse(x => new StringWrapper(x.RawOutput));
        resultAsync = (await SampleRequests.sampleRequestAsync.CompleteAsync()).Parse(x => new StringWrapper(x.RawOutput));
    }

    [Test]
    public void TestBranchingConditional()
    {
        var (passed, failed) = result.ThenBranchingConditional(x => x.RawOutput.StartsWith("yes"),
            x => GetOutput());

        Assert.That(passed, Is.EqualTo(input.Where(x => x.StartsWith("yes"))));
        Assert.That(failed, Is.EqualTo(input.Where(x => !x.StartsWith("yes"))));
    }

    public IEnumerable<LLMRequest> GetOutput()
    {
        foreach (int i in Enumerable.Range(0, input.Length))
        {
            yield return new LLMRequest(
                SampleRequests.sampleRequest.Prompt,
                SampleRequests.sampleRequest.LLMConfig,
                new SayGivenInputConnection(input),
                SampleRequests.sampleRequest.RequestConfig);
        }
    }

    [Test]
    public async Task TestBranchingConditionalAsync()
    {

        var (passed, failed) = await resultAsync.ThenBranchingConditionalAsync(x => x.RawOutput.StartsWith("yes"),
            x => GetOutputForAsync());

        Assert.That(passed, Is.EqualTo(input.Where(x => x.StartsWith("yes"))));
        Assert.That(failed, Is.EqualTo(input.Where(x => !x.StartsWith("yes"))));
    }

    public IEnumerable<LLMRequestAsync> GetOutputForAsync()
    {
        foreach (int i in Enumerable.Range(0, input.Length))
        {
            yield return new LLMRequestAsync(
                SampleRequests.sampleRequest.Prompt,
                SampleRequests.sampleRequest.LLMConfig,
                new SayGivenInputConnectionAsync(input),
                SampleRequests.sampleRequestAsync.RequestConfig);
        }
    }
}
