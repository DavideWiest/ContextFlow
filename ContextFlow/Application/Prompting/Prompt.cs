using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContextFlow.Application.TextUtil;
using ContextFlow.Infrastructure.Formatter;

namespace ContextFlow.Application.Prompting;

public class Prompt
{

    protected string PromptAction;

    public List<Attachment> Attachments { get; } = new();

    public Prompt(string action)
    {
        PromptAction = action;
    }
    public Prompt UsingAttachments(IEnumerable<Attachment> attachments)
    {
        Attachments.AddRange(attachments);
        return this;
    }

    public Prompt UsingAttachment(Attachment attachment)
    {
        Attachments.Add(attachment);
        return this;
    }

    public Prompt UsingAttachment(string? name, string content)
    {
        Attachments.Add(new Attachment(name, content, false));
        return this;
    }

    public Prompt UsingAttachmentInline(string? name, string content)
    {
        Attachments.Add(new Attachment(name, content, true));
        return this;
    }

    public Prompt UsingAttachment<T>(string? name, T content, ToStringConverter<T> converter)
    {
        Attachments.Add(new Attachment(name, converter.Convert(content), false));
        return this;
    }

    public Prompt UsingAttachmentInline<T>(string? name, T content, ToStringConverter<T> converter)
    {
        Attachments.Add(new Attachment(name, converter.Convert(content), true));
        return this;
    }

    public Prompt UpsertingAttachment(Attachment attachment)
    {
        var a = Attachments.FirstOrDefault(a => a.Name == attachment.Name);
        if (a != null)
        {
            a.Content = attachment.Content;
        }
        else
        {
            UsingAttachment(attachment);
        }
        return this;
    }

    public Prompt UsingOutputDescription(string description)
    {
        UpsertingAttachment(new Attachment("Output format", description, true));
        return this;
    }

    public virtual string ToPlainText()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    public override string ToString()
    {
        string attachmentstr = String.Join(", ", Attachments);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=[ {attachmentstr} ])";
    }

    /// <summary>
    /// Shallow copy using memberwise-clone
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Prompt Clone()
    {
        return (Prompt)MemberwiseClone();
    }
}
