using ContextFlow.Infrastructure.Formatter;

namespace ContextFlow.Application.Prompting;

/// <summary>
/// A prompt that can be formatted. Requires a formatter. 
/// Has an option ThowExceptionOnUnfilled to determine if an exception is thrown if the string representation is requested but not all placeholders have a value yet
/// </summary>
public class FormattablePrompt : Prompt
{

    /// <summary>
    /// Will throw an exception if not all placeholders have a corresponding value to be replaced with.
    /// </summary>
    private readonly bool ThowExceptionOnUnfilled = true;
    
    private Dictionary<string, object> FormatParameters = new();

    private readonly CFFormatter Formatter;

    public FormattablePrompt(string action) : base(action)
    {
        Formatter = new SmartFormatterFmtr();
    }

    /// <summary>
    /// throwExceptionOnUnfilled: Will throw an exception if not all placeholders have a corresponding value to be replaced with.
    /// </summary>
    public FormattablePrompt(string action, bool throwExceptionOnUnfilled) : base(action)
    {
        ThowExceptionOnUnfilled = throwExceptionOnUnfilled;
        Formatter = new SmartFormatterFmtr();
    }

    public FormattablePrompt(string action, CFFormatter formatter, bool thowExceptionOnUnfilled) : base(action)
    {
        ThowExceptionOnUnfilled = thowExceptionOnUnfilled;
        Formatter = formatter;
    }

    /// <summary>
    /// Set the value of a placeholder
    /// </summary>
    /// <param name="placeholder"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public FormattablePrompt UsingValue(string placeholder, string value)
    {
        FormatParameters[placeholder] = value;
        return this;
    }

    /// <summary>
    /// Returns the string representation of the prompt
    /// </summary>
    /// <returns></returns>
    public override string ToPlainText()
    {
        string plainText = Formatter.Format(ToPlainTextUnformatted(), FormatParameters);

        return plainText;
    }

    /// <summary>
    /// Returns an unformatted string representation of the prompt
    /// </summary>
    /// <returns></returns>
    public string ToPlainTextUnformatted()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    /// <summary>
    /// Determines if the prompt is valid, which means all placeholders have values set.
    /// </summary>
    /// <returns></returns>
    public bool IsValid()
    {
        List<string> _;
        return IsValid(out _);
    }

    /// <summary>
    /// Determines if the prompt is valid and saves the undefined placeholders.
    /// </summary>
    /// <param name="undefinedPlaceholderValues"></param>
    /// <returns></returns>
    public bool IsValid(out List<string> undefinedPlaceholderValues)
    {
        undefinedPlaceholderValues = Formatter.GetUndefinedPlaceholderValues(ToPlainTextUnformatted(), FormatParameters);
        return undefinedPlaceholderValues.Count == 0;
    }

    /// <summary>
    /// Calls IsValid and throws a FormatException if it is not valid.
    /// </summary>
    /// <exception cref="FormatException"></exception>
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
