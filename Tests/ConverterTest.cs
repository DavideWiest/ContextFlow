
namespace Tests;

using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;

public class ConverterTest
{

    class TestClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Num { get; set; }
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
        Assert.AreEqual("- A\n- B\n- C", new StandardConverter<List<string>>().FromObject(new() { "A", "B", "C" }, null));
    }
    [Test]
    public void TestToStringDict()
    {
        Assert.AreEqual("- A\n- B\n- C", new StandardConverter<Dictionary<string, string>>().FromObject(new() { { "A", "1"}, { "B", "2" }, { "C", "3" } }, null));
    }

    [Test]
    public void TestObject()
    {
        var test = new TestClass();
        test.Name = "John";
        test.Description = "Test description";
        test.Num = 3.14;
        Assert.AreEqual("# test\nName: John\nDescription: Test description\nNum: 3.14\n## InnerTestClass\nInnerValue: 4", new StandardConverter<TestClass>().FromObject(test, null));
    }


}