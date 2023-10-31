
namespace ContextFlow.Infrastructure.Logging;
using Serilog;

/// <summary>
/// Contextflow interface of a serilog logger. The SLogger representing the serilog-logger can be overwritten in a child implementation to customize it.
/// </summary>
public class CFSerilogLogger : CFLogger
{
    protected readonly ILogger SLogger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#endif
    .WriteTo.File("Logs/log_.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

    public override void Debug(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Debug(messageTemplate, propertyValues);
    }

    public override void Information(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Information(messageTemplate, propertyValues);
    }

    public override void Warning(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Warning(messageTemplate, propertyValues);
    }

    public override void Error(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Error(messageTemplate, propertyValues);
    }
}
