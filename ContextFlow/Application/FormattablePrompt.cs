using ContextFlow.Infrastructure.Formatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application;

public class FormattablePrompt : Prompt
{

    /// <summary>
    /// Will throw an exception if the prompt is invalid. A prompt is invalid if:
    /// - not all placeholders have a corresponding value to be replaced with
    /// </summary>
    private bool ThowExceptionOnUnfilled = true;

    private Dictionary<string, object> FormatParameters = new();

    private Formatter Formatter;

    public FormattablePrompt(string action) : base(action)
    {
        Formatter = new SmartFormatterFmtr();
    }
    public FormattablePrompt(string action, bool thowExceptionOnUnfilled) : base(action)
    {
        ThowExceptionOnUnfilled = thowExceptionOnUnfilled;
        Formatter = new SmartFormatterFmtr();
    }

    public FormattablePrompt(string action, bool thowExceptionOnUnfilled, Formatter formatter) : base(action)
    {
        ThowExceptionOnUnfilled = thowExceptionOnUnfilled;
        Formatter = formatter;
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

    public string ToPlainTextFormatted()
    {
        string plainText = Formatter.Format(ToPlainText(), FormatParameters);

        return plainText;
    }

    public void Validate()
    {
        List<string> undefinedPlaceholderValues = Formatter.GetUndefinedPlaceholderValues(ToPlainText(), FormatParameters);
        if (undefinedPlaceholderValues.Count > 0 && ThowExceptionOnUnfilled)
        {
            throw new FormatException("Prompt is invalid: Not all placeholders can be replaced with their corresponding value. Use the UsingValue- or SetValue-methods to set the values. Undefined values for the placeholders: " + string.Join(", ", undefinedPlaceholderValues));
        }
    }

    public Prompt UsingValue(string placeholder, dynamic value)
    {
        SetValue(placeholder, value);
        return this;
    }

    public void SetValue(string placeholder, dynamic value)
    {
        FormatParameters[placeholder] = promptConverter.FromDynamic(value);
    }

    public override string ToString()
    {
        string paramStr = string.Join(", ", FormatParameters);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=\"{Attachments}\", FormatParameters=\"{paramStr}\")";
    }

}
