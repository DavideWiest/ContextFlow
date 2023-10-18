using DocGenApi.Modules.Logging;

namespace DocGenApi.Modules.Logging;
using Serilog;
using Serilog.Events;

public static class Log
{
    private static readonly ILogger SLogger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#endif
    .WriteTo.File("Logs/log_.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


    public static void Verbose(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Verbose(messageTemplate, propertyValues);
    }

    public static void Debug(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Debug(messageTemplate, propertyValues);
    }

    public static void Information(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Information(messageTemplate, propertyValues);
    }

    public static void Warning(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Warning(messageTemplate, propertyValues);
    }

    public static void Error(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Error(messageTemplate, propertyValues);
    }

    public static void Fatal(string messageTemplate, params object[] propertyValues)
    {
        SLogger.Fatal(messageTemplate, propertyValues);
    }
}
