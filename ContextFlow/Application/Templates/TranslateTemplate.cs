using ContextFlow.Application.Prompting;
using ContextFlow.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        prompt.UsingAttachmentInline("Target language", TargetLanguage);
        prompt.UsingAttachment("Input text", InputText);
        
        return prompt;
    }
}
