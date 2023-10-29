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
    public static RequestLoader insensitiveLoader = new JsonRequestLoader(SampleRequests.sampleRequestCorrectResultFile, false);
    public static RequestLoaderAsync insensitiveLoaderAsync = new JsonRequestLoaderAsync(SampleRequests.sampleRequestCorrectResultFile, false);


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
    public void TestMatchExistsAndLoadMatchInsensitive()
    {
        bool matchexists = insensitiveLoader.MatchExists(SampleRequests.sampleRequest);
        RequestResult result = insensitiveLoader.LoadMatch(SampleRequests.sampleRequest);
        RequestResult comparisonResult = SampleRequests.sampleRequest.Complete();
        Assert.That(matchexists, Is.True);
        Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchAsync()
    {
        bool matchexists = await loaderAsync.MatchExistsAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync result = await loaderAsync.LoadMatchAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync comparisonResult = await SampleRequests.sampleRequestAsync.Complete();
        Assert.That(matchexists, Is.True);
        Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchInsensitiveAsync()
    {
        bool matchexists = await insensitiveLoaderAsync.MatchExistsAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync result = await insensitiveLoaderAsync.LoadMatchAsync(SampleRequests.sampleRequestAsync);
        RequestResultAsync comparisonResult = await SampleRequests.sampleRequestAsync.Complete();
        Assert.That(matchexists, Is.True);
        Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
    }

}