using ContextFlow.Application.Storage.Json;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Storage.Async.Json;
using Tests.Sample;
using ContextFlow.Application.Storage.Async;

namespace Tests.Storage;

public class TestRequestSaver
{
    static readonly string testFile1 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/LoaderStorageTest.json";
    static readonly string testFile2 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/LoaderAsyncStorageTest.json";
    readonly RequestSaver saver = new JsonRequestSaver(testFile1, true);
    readonly RequestSaverAsync saverAsync = new JsonRequestSaverAsync(testFile2, true);

    [OneTimeTearDown]
    public void Cleanup()
    {
        File.Delete(testFile1);
        File.Delete(testFile2);
    }

    [Test]
    public void TestSaveRequest()
    {
        saver.SaveRequest(SampleRequests.sampleRequest, SampleRequests.sampleRequest.Complete());
        Assert.That(
            File.ReadAllText(SampleRequests.sampleRequestCorrectResultFile).Split("timestamp")[0],
            Is.EqualTo(File.ReadAllText(testFile1).Split("timestamp")[0]));
    }

    [Test]
    public async Task TestSaveRequestAsync()
    {
        await saverAsync.SaveRequestAsync(SampleRequests.sampleRequestAsync, await SampleRequests.sampleRequestAsync.Complete());
        Assert.That(
            File.ReadAllText(SampleRequests.sampleRequestCorrectResultFile).Split("timestamp")[0],
            Is.EqualTo(File.ReadAllText(testFile1).Split("timestamp")[0]));
    }

}
