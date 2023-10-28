using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Storage;
using Tests.Sample;

namespace Tests.Storage;

public class TestRequestLoader
{
    public static RequestLoader loader = new JsonRequestLoader(SampleRequests.sampleRequestCorrectResultFile);
    public static RequestLoaderAsync loaderAsync = new JsonRequestLoaderAsync(SampleRequests.sampleRequestCorrectResultFile);

    [Test]
    public void TestMatchExistsAndLoadMatch()
    {
        bool matchexists = loader.MatchExists(SampleRequests.sampleRequest);
        RequestResult result = loader.LoadMatch(SampleRequests.sampleRequest);
        RequestResult comparisonResult = SampleRequests.sampleRequest.Complete();
        Assert.That(matchexists, Is.True);
        Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchAsync()
    {
        bool matchexists = await loaderAsync.MatchExistsAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync result = await loaderAsync.LoadMatchAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync comparisonResult = await SampleRequests.sampleRequestAsync.CompleteAsync();
        Assert.That(matchexists, Is.True);
        Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
    }

}