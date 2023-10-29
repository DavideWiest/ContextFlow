using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Result;
using Tests.Fakes;
using Tests.Fakes.Async;
using Tests.Sample;

namespace Tests.Request;


public class RequestResultTest
{
    RequestResult result;
    RequestResult resultAsync;

    string[] input = new string[] { "yes", "no", "yes", "no", "yes" };

    [SetUp]
    public async Task Setup()
    {
        result = SampleRequests.sampleRequest.Complete();
        resultAsync = await SampleRequests.sampleRequestAsync.Complete();
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

        var (passed, failed) = await resultAsync.AsyncActions.ThenBranchingConditionalAsync(x => x.RawOutput.StartsWith("yes"),
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
