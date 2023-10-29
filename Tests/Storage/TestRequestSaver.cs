using ContextFlow.Application.Storage.Json;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Storage.Json.Async;
using Tests.Sample;

namespace Tests.Storage;

public class TestRequestSaver
{
    static string testFile1 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/LoaderStorageTest.json";
    static string testFile2 = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/IOTestFiles/LoaderAsyncStorageTest.json";

    RequestSaver saver = new JsonRequestSaver(testFile1);
    RequestSaverAsync saverAsync = new JsonRequestSaverAsync(testFile2);

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
