using ContextFlow.Application.Prompting;
namespace ContextFlow.Application.Templates;

public class ExpandTemplate : CFTemplate
{
    public override string Action { get; } = "Expand the given text into approximately the target length.";

    private string TargetLength;
    private string InputText;

    public ExpandTemplate(string inputText, string targetLength)
    {
        TargetLength = targetLength;
        InputText = inputText;
    }

    protected override Prompt ConfigurePrompt(Prompt prompt)
    {
        prompt.UsingAttachmentInline("Target length", TargetLength);
        prompt.UsingAttachment("Text", InputText);

        return prompt;
    }
}
