using ContextFlow.Infrastructure.Formatter;

namespace ContextFlow.Application.Prompting;

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

    public FormattablePrompt(string action, bool throwExceptionOnUnfilled) : base(action)
    {
        ThowExceptionOnUnfilled = throwExceptionOnUnfilled;
        Formatter = new SmartFormatterFmtr();
    }

    public FormattablePrompt(string action, Formatter formatter, bool thowExceptionOnUnfilled) : base(action)
    {
        ThowExceptionOnUnfilled = thowExceptionOnUnfilled;
        Formatter = formatter;
    }
    
    public FormattablePrompt UsingValue(string placeholder, string value)
    {
        FormatParameters[placeholder] = value;
        return this;
    }

    public override string ToPlainText()
    {
        string plainText = Formatter.Format(ToPlainTextUnformatted(), FormatParameters);

        return plainText;
    }

    public string ToPlainTextUnformatted()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    public bool IsValid()
    {
        List<string> _;
        return IsValid(out _);
    }

    public bool IsValid(out List<string> undefinedPlaceholderValues)
    {
        undefinedPlaceholderValues = Formatter.GetUndefinedPlaceholderValues(ToPlainTextUnformatted(), FormatParameters);
        return undefinedPlaceholderValues.Count == 0;
    }

    public void Validate()
    {
        IsValid(out List<string> undefinedPlaceholderValues);

        if (undefinedPlaceholderValues.Count > 0 && ThowExceptionOnUnfilled)
        {
            throw new FormatException("Prompt is invalid: Not all placeholders can be replaced with their corresponding value. Use the UsingValue-method to set the values. Undefined values for the placeholders: " + string.Join(", ", undefinedPlaceholderValues));
        }
    }

    public override string ToString()
    {
        string paramStr = string.Join(", ", FormatParameters);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=\"{Attachments}\", FormatParameters=\"{paramStr}\")";
    }

}
