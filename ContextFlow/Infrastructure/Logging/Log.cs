
namespace ContextFlow.Infrastructure.Logging;
using Serilog;


public class CFDefaultLogger : CFLogger
{
    private readonly ILogger SLogger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#endif
    .WriteTo.File("Logs/log_.txt", rollingInterval: RollingInterval.Day)
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

    public override void Fatal(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Fatal(messageTemplate, propertyValues);
    }
}
