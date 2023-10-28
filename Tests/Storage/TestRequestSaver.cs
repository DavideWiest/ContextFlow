
using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Request;
using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Storage;
using ContextFlow.Domain;
using Tests.Fakes;
using Tests.Sample;

namespace Tests.Storage;

public class TestRequestSaver
{
    static string testFile1 = "IOTestFiles/LoaderStorageTest.json";
    static string testFile2 = "IOTestFiles/LoaderAsyncStorageTest.json";

    RequestSaver saver = new JsonRequestSaver(testFile1);
    RequestSaverAsync saverAsync = new JsonRequestSaverAsync(testFile2);

    [OneTimeTearDown]
    public void Cleanup()
    {
        //File.Delete(testFile1);
        //File.Delete(testFile2);
    }

    [Test]
    public void TestSaveRequest()
    {
        saver.SaveRequest(SampleRequests.sampleRequest, SampleRequests.sampleRequest.Complete());
        Assert.That(File.ReadAllText(SampleRequests.sampleRequestCorrectResultFile), Is.EqualTo(File.ReadAllText(testFile1)));
    }

    [Test]
    public async Task TestSaveRequestAsync()
    {
        await saverAsync.SaveRequestAsync(SampleRequests.sampleRequestAsync, await SampleRequests.sampleRequestAsync.CompleteAsync());
        Assert.That(File.ReadAllText(SampleRequests.sampleRequestCorrectResultFile), Is.EqualTo(File.ReadAllText(testFile1)));
    }

}
