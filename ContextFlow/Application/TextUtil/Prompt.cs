using SmartFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;



namespace ContextFlow.Application.TextUtil;

public class Prompt
{

    private string PromptAction;

    private List<Attachment> Attachments = new();
    private Dictionary<string, string> FormatParameters = new();

    /// <summary>
    /// Will throw an exception if the prompt is invalid. A prompt is invalid if:
    /// - not all placeholders have a corresponding value to be replaced with
    /// Set this to false
    /// </summary>
    private bool ThrowExceptionOnInvalid = true;

    private SmartFormatter formatter = Smart.CreateDefaultSmartFormat();

    public Prompt(string action)
    {
        PromptAction = action;
    }
    public Prompt(string action, bool thowExceptionOnUnfilled)
    {
        PromptAction = action;
        ThrowExceptionOnInvalid = thowExceptionOnUnfilled;
    }

    public string ToPlainText()
    {
        string plainText = formatter.Format(ToPlainTextUnformatted());

        return plainText;
    }

    public override string ToString()
    {
        string paramStr = String.Join(", ", FormatParameters);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=\"{Attachments}\", FormatParameters=\"{paramStr}\")";
    }

    public void Validate()
    {
        List<string> undefinedPlaceholderValues = GetUndefinedPlaceholderValues();
        if (undefinedPlaceholderValues.Count > 0 && ThrowExceptionOnInvalid)
        {
            throw new FormatException("Prompt is invalid: Not all placeholders can be replaced with their value. Use the ");
        }
    }

    private List<string> GetUndefinedPlaceholderValues()
    {
        var placeholderMatches = Regex.Matches(ToPlainTextUnformatted(), @"\{([^{}\s]+)\}");
        var unreplacedPlaceholders = new List<string>();

        foreach (Match match in placeholderMatches)
        {
            string placeholder = match.Groups[1].Value;

            // Check if the placeholder exists in the data
            if (FormatParameters[placeholder] == null)
            {
                unreplacedPlaceholders.Add(placeholder);
            }
        }

        return unreplacedPlaceholders;
    }

    public string ToPlainTextUnformatted()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + String.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }
}
