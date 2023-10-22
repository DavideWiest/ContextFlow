using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SmartFormat;

namespace ContextFlow.Infrastructure.Formatter;



internal class SmartFormatterFmtr: Formatter
{
    SmartFormatter formatter = Smart.CreateDefaultSmartFormat();

    public override string Format(string str, Dictionary<string, object> data)
    {
        return formatter.Format(str, data);
    }

    public override List<string> GetUndefinedPlaceholderValues(string str, Dictionary<string, object> placeholderValues)
    {
        var placeholderMatches = Regex.Matches(str, @"\{([^{}\s]+)\}");
        var unreplacedPlaceholders = new List<string>();

        foreach (Match match in placeholderMatches)
        {
            string placeholder = match.Groups[1].Value;

            // Check if the placeholder exists in the placeholderValues
            if (!placeholderValues.ContainsKey(placeholder))
            {
                unreplacedPlaceholders.Add(placeholder);
            }
        }

        return unreplacedPlaceholders;
    }
}
