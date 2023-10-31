using ContextFlow.Application.Storage.Json;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Storage.Async.Json;
using Tests.Sample;
using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Fakes.Async;

namespace Tests.Storage;

public class TestRequestSaver
{

    private static LLMRequest sampleRequest = new LLMRequest(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnection(), new RequestConfig());
    private static LLMRequestAsync sampleRequestAsync = new LLMRequestAsync(new Prompt("Test"), new LLMConfig("gpt-3.5-turbo", 100, 50), new SayHiConnectionAsync(), new RequestConfigAsync());

    private static string sampleRequestCorrectResultFile = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/CorrectSampleRequestSaved.json";
    static readonly string testFile1 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/SaverStorageTest.json";
    static readonly string testFile2 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/SaverAsyncStorageTest.json";
    
    readonly RequestSaver saver = new JsonRequestSaver(testFile1, true);
    readonly RequestSaverAsync saverAsync = new JsonRequestSaverAsync(testFile2, true);

    [OneTimeTearDown]
    public void Cleanup()
    {
        //File.Delete(testFile1);
        //File.Delete(testFile2);
    }

    [Test]
    public void TestSaveRequest()
    {
        saver.SaveRequest(sampleRequest, sampleRequest.Complete());
        Assert.That(
            File.ReadAllText(sampleRequestCorrectResultFile).Split("timestamp")[0],
            Is.EqualTo(File.ReadAllText(testFile1).Split("timestamp")[0]));
    }

    [Test]
    public async Task TestSaveRequestAsync()
    {
        await saverAsync.SaveRequestAsync(sampleRequestAsync, await sampleRequestAsync.Complete());
        Assert.That(
            File.ReadAllText(sampleRequestCorrectResultFile).Split("timestamp")[0],
            Is.EqualTo(File.ReadAllText(testFile1).Split("timestamp")[0]));
    }

}
