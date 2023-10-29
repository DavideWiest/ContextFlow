using ContextFlow.Application.Prompting;

namespace ContextFlow.Application.Templates;

public class TranslateTemplate : CFTemplate
{
    public override string Action { get; } = "Translate the text \"Input text\" into the target language. Return only the translation.";

    private string TargetLanguage;
    private string InputText;

    public TranslateTemplate(string inputText, string targetLanguage)
    {
        TargetLanguage = targetLanguage;
        InputText = inputText;
    }

    protected override Prompt ConfigurePrompt(Prompt prompt)
    {
        prompt.UsingAttachment(new Attachment("Target language", TargetLanguage, true));
        prompt.UsingAttachment(new Attachment("Input text", InputText, false));
        
        return prompt;
    }
}
