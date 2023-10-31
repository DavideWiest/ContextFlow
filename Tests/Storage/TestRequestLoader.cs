using ContextFlow.Application.Result;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Storage.Json;
using ContextFlow.Application.Storage.Async.Json;
using Tests.Sample;
using ContextFlow.Application.Storage.Async;

namespace Tests.Storage;

public class TestRequestLoader
{
    private static readonly RequestLoader loader = new JsonRequestLoader(SampleRequests.sampleRequestCorrectResultFile);
    private static readonly RequestLoaderAsync loaderAsync = new JsonRequestLoaderAsync(SampleRequests.sampleRequestCorrectResultFile);
    private static readonly RequestLoader insensitiveLoader = new JsonRequestLoader(SampleRequests.sampleRequestCorrectResultFile, false);
    private static readonly RequestLoaderAsync insensitiveLoaderAsync = new JsonRequestLoaderAsync(SampleRequests.sampleRequestCorrectResultFile, false);


    [Test]
    public void TestMatchExistsAndLoadMatch()
    {
        bool matchexists = loader.MatchExists(SampleRequests.sampleRequest);
        RequestResult result = loader.LoadMatch(SampleRequests.sampleRequest);
        RequestResult comparisonResult = SampleRequests.sampleRequest.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public void TestMatchExistsAndLoadMatchInsensitive()
    {
        bool matchexists = insensitiveLoader.MatchExists(SampleRequests.sampleRequest);
        RequestResult result = insensitiveLoader.LoadMatch(SampleRequests.sampleRequest);
        RequestResult comparisonResult = SampleRequests.sampleRequest.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchAsync()
    {
        bool matchexists = await loaderAsync.MatchExistsAsync(SampleRequests.sampleRequestAsync);
        RequestResult result = await loaderAsync.LoadMatchAsync(SampleRequests.sampleRequestAsync);
        RequestResult comparisonResult = await SampleRequests.sampleRequestAsync.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchInsensitiveAsync()
    {
        bool matchexists = await insensitiveLoaderAsync.MatchExistsAsync(SampleRequests.sampleRequestAsync);
        RequestResult result = await insensitiveLoaderAsync.LoadMatchAsync(SampleRequests.sampleRequestAsync);
        RequestResult comparisonResult = await SampleRequests.sampleRequestAsync.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }
}