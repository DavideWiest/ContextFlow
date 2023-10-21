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

        Assert.That(output, Is.EqualTo("Test, test, test"));
    }

    [Test]
    public void TestSectionMerger()
    {
        Assert.Pass();
    }
}