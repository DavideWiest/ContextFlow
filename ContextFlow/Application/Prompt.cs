﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContextFlow.Application.TextUtil;
using ContextFlow.Infrastructure.Formatter;

namespace ContextFlow.Application;

public class Prompt
{

    protected string PromptAction;

    public List<Attachment> Attachments = new();

    public Prompt(string action)
    {
        PromptAction = action;
    }

    public Prompt UsingAttachment(string name, string content)
    {
        AddAttachment(name, content);
        return this;
    }

    public void AddAttachment(string name, string content)
    {
        Attachments.Add(new Attachment(name, content, false));
    }

    public Prompt UsingAttachmentInline(string name, string content)
    {
        AddAttachmentInline(name, content);
        return this;
    }

    public void AddAttachmentInline(string name, string content)
    {
        Attachments.Add(new Attachment(name, content, true));
    }

    public Prompt UsingAttachment<T>(string name, T content, CFConverter<T> converter, dynamic? converterData = null)
    {
        Attachments.Add(new Attachment(name, converter.FromObject(content, converterData), false));
        return this;
    }

    public Prompt UsingAttachmentInline<T>(string name, T content, CFConverter<T> converter, dynamic? converterData = null)
    {
        Attachments.Add(new Attachment(name, converter.FromObject(content, converterData), true));
        return this;
    }

    public Prompt UsingOutputDescription(string description)
    {
        SetOutputDescription(description);
        return this;
    }

    public void SetOutputDescription(string description)
    {
        var a = Attachments.FirstOrDefault(a => a.Name == "Output format");
        if (a != null)
        {
            a.Content = description;
        } else
        {
            AddAttachmentInline("Output format", description);
        }
    }

    public virtual string ToPlainText()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    public override string ToString()
    {
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=\"{Attachments}\")";
    }

    /// <summary>
    /// Shallow copy using ICloneable
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Prompt Clone()
    {
        return (Prompt)MemberwiseClone();
    }

    //protected void ThrowExceptionIfNoConverter()
    //{
    //    if (promptConverter == null)
    //    {
    //        throw new InvalidOperationException("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter or SetConverter to fix it.");
    //    }
    //}
}
