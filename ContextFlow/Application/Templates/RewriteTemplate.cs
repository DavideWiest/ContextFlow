using ContextFlow.Application.Prompting;

namespace ContextFlow.Application.Templates;

public class RewriteTemplate : CFTemplate
{
    public override string Action { get; } = "Rewrite the given text into the target style.";

    private string TargetStyle;
    private string InputText;

    public RewriteTemplate(string inputText, string targetStyle)
    {
        TargetStyle = targetStyle;
        InputText = inputText;
    }

    protected override Prompt ConfigurePrompt(Prompt prompt)
    {
        prompt.UsingAttachment(new Attachment("Target style", TargetStyle, true));
        prompt.UsingAttachment(new Attachment("Text", InputText, false));

        return prompt;
    }
}
