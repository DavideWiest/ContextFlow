namespace Tests.Prompting;

using ContextFlow.Application.Prompting;
using ContextFlow.Infrastructure.Providers.OpenAI;

public class PromptTest
{

    private OpenAIChatConnection llmcon = new();

    private Prompt baseTestPrompt = new("Test test");


    [Test]
    public void TestPromptAction()
    {
        Assert.That(baseTestPrompt.ToPlainText(), Is.EqualTo("Test test"));
    }

    [Test]
    public void TestPromptAttachments()
    {
        var prompt = baseTestPrompt
            .Clone()
            .UsingAttachment(new Attachment("Test attachment", "-> Test attachment Content", false))
            .UsingAttachment(new Attachment("Attachment 2", "Inline", true));
        Assert.That(prompt.ToPlainText(), Is.EqualTo("Test test\n\nTest attachment: \n-> Test attachment Content\n\nAttachment 2: Inline"));
    }

    [Test]
    public void TestCloning()
    {
        Prompt prompt = new Prompt("Test prompt").Clone();
        Assert.That(typeof(Prompt), Is.EqualTo(prompt.GetType()));
    }

    [Test]
    public void TestFormatter()
    {
        var promptstr = new FormattablePrompt("{placeholder}").UsingValue("placeholder", "hi").ToPlainText();
        Assert.That(promptstr, Is.EqualTo("hi"));
    }

    [Test]
    public void TestFormatterValidation()
    {
        var isvalid = new FormattablePrompt("{placeholder}", false).IsValid();
        Assert.That(isvalid, Is.EqualTo(false));
    }

    [Test]
    public void TestSetOutputDescription()
    {
        var promptstr = new Prompt("A").UsingOutputDescription("OutputDescription").ToPlainText();
        Assert.That(promptstr, Is.EqualTo("A\n\nOutput format: OutputDescription"));
    }

    [Test]
    public void TestOutputDescription()
    {
        var promptstr = new Prompt("A").UsingOutputDescription("OutputDescription").UsingOutputDescription("new description").ToPlainText();

        Assert.That(promptstr, Is.EqualTo("A\n\nOutput format: new description"));
    }


}