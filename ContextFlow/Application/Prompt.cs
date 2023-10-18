using System;
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

    private string PromptAction;
    private CFConverter promptConverter;

    public List<Attachment> Attachments = new();
    private Dictionary<string, object> FormatParameters = new();

    /// <summary>
    /// Will throw an exception if the prompt is invalid. A prompt is invalid if:
    /// - not all placeholders have a corresponding value to be replaced with
    /// Set this to false
    /// </summary>
    private bool ThrowExceptionOnInvalid = true;

    private Formatter Formatter;

    public Prompt(string action)
    {
        PromptAction = action;
        Formatter = new SmartFormatterFmtr();
    }
    public Prompt(string action, bool thowExceptionOnUnfilled)
    {
        PromptAction = action;
        ThrowExceptionOnInvalid = thowExceptionOnUnfilled;
        Formatter = new SmartFormatterFmtr();
    }

    public Prompt(string action, bool thowExceptionOnUnfilled, Formatter formatter)
    {
        PromptAction = action;
        ThrowExceptionOnInvalid = thowExceptionOnUnfilled;
        Formatter = formatter;
    }

    public Prompt UsingConverter(CFConverter converter)
    {
        SetConverter(converter);
        return this;
    }

    public void SetConverter(CFConverter converter)
    {
        promptConverter = converter;
    }

    public Prompt UsingValue(string placeholder, string value)
    {
        SetValue(placeholder, value);
        return this;
    }

    public void SetValue(string placeholder, string value)
    {
        FormatParameters[placeholder] = value;
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

    public Prompt UsingValue(string placeholder, dynamic value)
    {
        SetValue(placeholder, value);
        return this;
    }

    public void SetValue(string placeholder, dynamic value)
    {
        CheckConverterExists();
        FormatParameters[placeholder] = promptConverter.FromDynamic(content);
    }

    public Prompt UsingAttachment(string name, dynamic content)
    {
        AddAttachment(name, content);
        return this;
    }

    public void AddAttachment(string name, dynamic content)
    {
        CheckConverterExists();
        Attachments.Add(new Attachment(name, promptConverter.FromDynamic(content), false));
    }

    public Prompt UsingAttachmentInline(string name, dynamic content)
    {
        AddAttachmentInline(name, content);
        return this;
    }

    public void AddAttachmentInline(string name, dynamic content)
    {
        CheckConverterExists();
        Attachments.Add(new Attachment(name, promptConverter.FromDynamic(content), true));
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

    public string ToPlainText()
    {
        string plainText = Formatter.Format(ToPlainTextUnformatted(), FormatParameters);

        return plainText;
    }

    public override string ToString()
    {
        string paramStr = string.Join(", ", FormatParameters);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=\"{Attachments}\", FormatParameters=\"{paramStr}\")";
    }

    public void Validate()
    {
        List<string> undefinedPlaceholderValues = Formatter.GetUndefinedPlaceholderValues(ToPlainTextUnformatted(), FormatParameters);
        if (undefinedPlaceholderValues.Count > 0 && ThrowExceptionOnInvalid)
        {
            throw new FormatException("Prompt is invalid: Not all placeholders can be replaced with their corresponding value. Use the UsingValue- or SetValue-methods to set the values. Undefined values for the placeholders: " + string.Join(", ", undefinedPlaceholderValues));
        }
    }

    public string ToPlainTextUnformatted()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    /// <summary>
    /// Shallow copy using ICloneable
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Prompt Clone()
    {
        return (Prompt)MemberwiseClone();
    }

    private void CheckConverterExists()
    {
        if (promptConverter == null)
        {
            throw new InvalidOperationException("Can't convert dynamic content to string when there is not converter defined. Use UsingConverter or SetConverter to fix it.");
        }
    }
}
