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
        // Arrange
        var merger = new SimpleSectionMerger(new() { { "A: ", "\n" }, { "B: ", " - " } });
        var inputs = new List<string> { "A: apple apple B: banana banana", "B: pineapple A: strawberry B: avocado" };

        // Act
        string output = merger.Merge(inputs);

        // Assert
        string expectedOutput = "A: apple apple\nstrawberry B: banana banana - pineapple - avocado";
        Assert.That(output, Is.EqualTo(expectedOutput));
    }
}