using ContextFlow.Application.Prompting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        prompt.UsingAttachmentInline("Target style", TargetStyle);
        prompt.UsingAttachment("Text", InputText);

        return prompt;
    }
}
