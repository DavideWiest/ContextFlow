using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using Tests.Fakes;
using Tests.Fakes.Async;
using Tests.Util;
using Tests.Sample;
using ContextFlow.Application.Result;

namespace Tests.Request;


public class ParsedRequestResultTest
{
    ParsedRequestResult<StringWrapper> result;
    ParsedRequestResult<StringWrapper> resultAsync;

    string[] input = new string[] { "yes", "no", "yes", "no", "yes" };

    [SetUp]
    public async Task Setup()
    {
        result = SampleRequests.sampleRequest.Complete().Actions.Parse(x => new StringWrapper(x.RawOutput));
        resultAsync = (await SampleRequests.sampleRequestAsync.Complete()).Actions.Parse(x => new StringWrapper(x.RawOutput));
    }

    [Test]
    public void TestBranchingConditional()
    {
        var (passed, failed) = result.Actions.ThenBranchingConditional(x => x.RawOutput.StartsWith("yes"),
            x => GetOutput());

        Assert.That(
            string.Join(",", passed.Select(x => x.RawOutput).ToArray()),
            Is.EqualTo(string.Join(",", input.Where(x => x.StartsWith("yes")).ToArray()))
        );
        Assert.That(
            string.Join(",", failed.Select(x => x.RawOutput).ToArray()),
            Is.EqualTo(string.Join(",", input.Where(x => !x.StartsWith("yes")).ToArray()))
        );
    }

    public IEnumerable<LLMRequest> GetOutput()
    {
        var con = new SayGivenInputConnection(input);
        foreach (int i in Enumerable.Range(0, input.Length))
        {
            yield return new LLMRequest(
                SampleRequests.sampleRequest.Prompt,
                SampleRequests.sampleRequest.LLMConfig,
                con,
                SampleRequests.sampleRequest.RequestConfig);
        }
    }

    [Test]
    public async Task TestBranchingConditionalAsync()
    {

        var (passed, failed) = await resultAsync.AsyncActions.ThenBranchingConditionalAsync(async x => x.RawOutput.StartsWith("yes"),
            x => GetOutputForAsync());

        Assert.That(
            string.Join(",", passed.Select(x => x.RawOutput).ToArray()),
            Is.EqualTo(string.Join(",", input.Where(x => x.StartsWith("yes")).ToArray()))
        );
        Assert.That(
            string.Join(",", failed.Select(x => x.RawOutput).ToArray()),
            Is.EqualTo(string.Join(",", input.Where(x => !x.StartsWith("yes")).ToArray()))
        );
    }

    public IEnumerable<LLMRequestAsync> GetOutputForAsync()
    {
        var con = new SayGivenInputConnectionAsync(input);
        foreach (int i in Enumerable.Range(0, input.Length))
        {
            yield return new LLMRequestAsync(
                SampleRequests.sampleRequest.Prompt,
                SampleRequests.sampleRequest.LLMConfig,
                con,
                SampleRequests.sampleRequestAsync.RequestConfig);
        }
    }
}
