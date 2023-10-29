using ContextFlow.Application.Prompting;

namespace ContextFlow.Application.Templates;

public class SummarizeTemplate : CFTemplate
{
    public override string Action { get; } = "Summarize the following text. Return only the summarization.";

    private string TargetLength;
    private string InputText;

    public SummarizeTemplate(string inputText, string targetLength)
    {
        TargetLength = targetLength;
        InputText = inputText;
    }

    protected override Prompt ConfigurePrompt(Prompt prompt)
    {
        prompt.UsingAttachmentInline("Length", TargetLength);
        prompt.UsingAttachment("Text", InputText);
        
        return prompt;
    }
}