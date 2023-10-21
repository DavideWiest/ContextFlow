
namespace Tests;

using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;

public class ConverterTest
{

    class TestClass
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public double Num { get; set; } = default!;
        public InnerTestClass InnerTestClass { get; set; } = new InnerTestClass();
        public TestClass() { }
    }

    class InnerTestClass
    {
        public int InnerValue = 4;
    }

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestToStringList()
    {
        Assert.That(new StandardConverter<List<string>>().FromObject(new() { "A", "B", "C" }, null), Is.EqualTo("- A\n- B\n- C"));
    }
    [Test]
    public void TestToStringDict()
    {
        Assert.That(new StandardConverter<Dictionary<string, string>>().FromObject(new() { { "A", "1"}, { "B", "2" }, { "C", "3" } }, null), Is.EqualTo("- A\n- B\n- C"));
    }

    [Test]
    public void TestObject()
    {
        var test = new TestClass();
        test.Name = "John";
        test.Description = "Test description";
        test.Num = 3.14;
        Assert.That(new StandardConverter<TestClass>().FromObject(test, null), Is.EqualTo("# test\nName: John\nDescription: Test description\nNum: 3.14\n## InnerTestClass\nInnerValue: 4"));
    }


}