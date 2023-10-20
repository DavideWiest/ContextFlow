namespace Tests;

using ContextFlow.Application.TextUtil;

public class TextMergerTest
{

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestFunctionMerger()
    {
        var splitter = new FunctionTextMerger(x => String.Join(", ", x));
        var output = splitter.Merge(new() { "Test", "test", "test"});

        Assert.AreEqual("Test, test, test", output);
    }

    [Test]
    public void TestSectionMerger()
    {
        Assert.Pass();
    }
}