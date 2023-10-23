using ContextFlow.Application.Prompting.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Prompting;

public static class PromptTemplates
{
    public static Prompt Aggregate(string text)
    {
        return new Prompt(ActionDescriptions.Aggregate).UsingAttachment(new Attachment(text, false));
    }
    public static Prompt Aggregate(IEnumerable<string> texts)
    {
        return new Prompt(ActionDescriptions.Aggregate).UsingAttachments(texts.Select(t => new Attachment(t, false)));
    }


}
