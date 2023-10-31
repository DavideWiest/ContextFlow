namespace ContextFlow.Infrastructure.Logging;

/// <summary>
/// A logger that is used inside an LLMRequest.
/// </summary>
public abstract class CFLogger
{
    public abstract void Debug(string messageTemplate, params object[] propertyValues);
    public abstract void Information(string messageTemplate, params object[] propertyValues);
    public abstract void Warning(string messageTemplate, params object[] propertyValues);
    public abstract void Error(string messageTemplate, params object[] propertyValues);
}
