using ContextFlow.Application.Storage.Json.Async;
using ContextFlow.Application.Strategy.Async;

namespace ContextFlow.Application.Request.Async;

public class RequestConfigAsync : RequestConfigBase<RequestConfigAsync>
{
    public List<IFailStrategyAsync> FailStrategies { get; private set; } = new();
    public RequestLoaderAsync? RequestLoaderAsync { get; private set; }
    public RequestSaverAsync? RequestSaverAsync { get; private set; }

    public RequestConfigAsync AddFailStrategy(IFailStrategyAsync failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    public RequestConfigAsync AddFailStrategyToTop(IFailStrategyAsync failStrategy)
    {
        FailStrategies.Insert(0, failStrategy);
        return this;
    }

    public RequestConfigAsync ResetFailStrategies()
    {
        FailStrategies.Clear();
        return this;
    }

    public RequestConfigAsync RemoveSelectedFailStrategies(Func<IFailStrategyAsync, bool> predicate)
    {
        FailStrategies = FailStrategies.Where(x => predicate(x)).ToList();
        return this;
    }

    public RequestConfigAsync UsingRequestSaver(RequestSaverAsync requestSaver)
    {
        RequestSaverAsync = requestSaver;
        return this;
    }

    public RequestConfigAsync UsingRequestLoader(RequestLoaderAsync requestLoader)
    {
        RequestLoaderAsync = requestLoader;
        return this;
    }
}
