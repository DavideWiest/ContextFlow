using ContextFlow.Application.Result;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Storage.Json;
using ContextFlow.Application.Storage.Async.Json;
using Tests.Sample;
using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Domain;
using Tests.Fakes.Async;
using Tests.Fakes;

namespace Tests.Storage;

public class TestRequestLoader
{
    private static LLMRequest sampleRequest = new LLMRequest(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnection(), new RequestConfig());
    private static LLMRequestAsync sampleRequestAsync = new LLMRequestAsync(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnectionAsync(), new RequestConfigAsync());

    private static string sampleRequestCorrectResultFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/CorrectSampleRequestSavedForLoader.json";

    private static readonly RequestLoader loader = new JsonRequestLoader(sampleRequestCorrectResultFile);
    private static readonly RequestLoaderAsync loaderAsync = new JsonRequestLoaderAsync(sampleRequestCorrectResultFile);
    private static readonly RequestLoader insensitiveLoader = new JsonRequestLoader(sampleRequestCorrectResultFile, false);
    private static readonly RequestLoaderAsync insensitiveLoaderAsync = new JsonRequestLoaderAsync(sampleRequestCorrectResultFile, false);


    [Test]
    public void TestMatchExistsAndLoadMatch()
    {
        bool matchexists = loader.MatchExists(sampleRequest);
        RequestResult result = loader.LoadMatch(sampleRequest);
        RequestResult comparisonResult = sampleRequest.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public void TestMatchExistsAndLoadMatchInsensitive()
    {
        bool matchexists = insensitiveLoader.MatchExists(sampleRequest);
        RequestResult result = insensitiveLoader.LoadMatch(sampleRequest);
        RequestResult comparisonResult = sampleRequest.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchAsync()
    {
        bool matchexists = await loaderAsync.MatchExistsAsync(sampleRequestAsync);
        RequestResult result = await loaderAsync.LoadMatchAsync(sampleRequestAsync);
        RequestResult comparisonResult = await sampleRequestAsync.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }

    [Test]
    public async Task TestMatchExistsAndLoadMatchInsensitiveAsync()
    {
        bool matchexists = await insensitiveLoaderAsync.MatchExistsAsync(sampleRequestAsync);
        RequestResult result = await insensitiveLoaderAsync.LoadMatchAsync(sampleRequestAsync);
        RequestResult comparisonResult = await sampleRequestAsync.Complete();
        Assert.Multiple(() =>
        {
            Assert.That(matchexists, Is.True);
            Assert.That(result.RawOutput == comparisonResult.RawOutput && result.FinishReason == comparisonResult.FinishReason);
        });
    }
}