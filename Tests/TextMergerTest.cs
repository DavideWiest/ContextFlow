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
        var merger = new FunctionTextMerger(x => String.Join(", ", x));
        var output = merger.Merge(new string[] { "Test", "test", "test" });

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