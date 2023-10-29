namespace ContextFlow.Infrastructure.Formatter;

public abstract class Formatter
{
    public abstract string Format(string str, Dictionary<string, object> data);
    public abstract List<string> GetUndefinedPlaceholderValues(string str, Dictionary<string, object> formatParams);
}
