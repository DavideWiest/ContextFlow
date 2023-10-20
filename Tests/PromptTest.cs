namespace Tests;

using ContextFlow.Application;
using ContextFlow.Infrastructure.Providers.OpenAI;

public class PromptTest
{

    private OpenAIChatConnection llmcon = new();

    private Prompt baseTestPrompt = new("Test test");

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestPromptAction()
    {
        Assert.AreEqual("Test test", baseTestPrompt.ToPlainText());
    }

    [Test]
    public void TestPromptAttachments()
    {
        var prompt = baseTestPrompt
            .Clone()
            .UsingAttachment("Test attachment", "-> Test attachment content")
            .UsingAttachmentInline("Attachment 2", "Inline");
        Assert.AreEqual("Test test\n\nTest attachment: \n-> Test attachment content\n\nAttachment 2: Inline", prompt.ToPlainText());
    }

    [Test]
    public void TestCloning()
    {
        Prompt prompt = new Prompt("Test prompt").Clone();
        Assert.AreEqual(prompt.GetType(), typeof(Prompt));
    }

    [Test]
    public void TestFormatter()
    {
        var promptstr = new FormattablePrompt("{placeholder}").UsingValue("placeholder", "hi").ToPlainText();
        Assert.AreEqual("hi", promptstr);
    }

    [Test]
    public void TestFormatterValidation()
    {
        var isvalid = new FormattablePrompt("{placeholder}", false).IsValid();
        Assert.AreEqual(false, isvalid);
    }

    [Test]
    public void TestSetOutputDescription()
    {
        var promptstr = new Prompt("A").UsingOutputDescription("OutputDescription").ToPlainText();
        Assert.AreEqual("A\n\nOutput format: OutputDescription", promptstr);
    }

    [Test]
    public void TestOutputDescription()
    {
        var promptstr = new Prompt("A").UsingOutputDescription("OutputDescription").UsingOutputDescription("new description").ToPlainText();

        Assert.AreEqual("A\n\nOutput format: new description", promptstr);
    }


}