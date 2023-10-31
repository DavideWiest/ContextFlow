namespace ContextFlow.Infrastructure.Formatter;

/// <summary>
/// A formatter used mainly for FormattablePrompt that formats a string based on predefined parameters at once.
/// </summary>
public abstract class CFFormatter
{
    public abstract string Format(string str, Dictionary<string, object> data);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <param name="formatParams">Link the placeholder to its content, which is its value</param>
    /// <returns>A list of undefined placeholders found inside the string</returns>
    public abstract List<string> GetUndefinedPlaceholderValues(string str, Dictionary<string, object> formatParams);
}
