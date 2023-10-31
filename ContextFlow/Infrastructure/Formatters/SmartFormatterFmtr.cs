using SmartFormat;
using System.Text.RegularExpressions;
using System.Linq;

namespace ContextFlow.Infrastructure.Formatter;



public partial class SmartFormatterFmtr : CFFormatter
{
    private readonly SmartFormatter formatter = Smart.CreateDefaultSmartFormat();

    public override string Format(string str, Dictionary<string, object> data)
    {
        return formatter.Format(str, data);
    }

    public override List<string> GetUndefinedPlaceholderValues(string str, Dictionary<string, object> placeholderValues)
    {
        var placeholderMatches = PlaceHolderMatchRegex().Matches(str);
        return (from Match match in placeholderMatches
                let placeholder = match.Groups[1].Value // Check if the placeholder exists in the placeholderValues
                where !placeholderValues.ContainsKey(placeholder)
                select placeholder).ToList();
    }

    [GeneratedRegex("\\{([^{}\\s]+)\\}")]
    private static partial Regex PlaceHolderMatchRegex();
}
