using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Strategy.Async;

namespace ContextFlow.Application.Request.Async;

/// <summary>
/// Configuration of a request. This configuration applies only to the LLMRequestAsync class, not any others.
/// </summary>
public class RequestConfigAsync : RequestConfigBase<RequestConfigAsync>
{
    /// <summary>
    /// A list of failstrategies, which will be used to catch and handle exceptions.
    /// </summary>
    public List<IFailStrategyAsync> FailStrategies { get; private set; } = new();

    public RequestLoaderAsync? RequestLoaderAsync { get; private set; }
    public RequestSaverAsync? RequestSaverAsync { get; private set; }

    /// <summary>
    /// Adds a fail strategy to the bottom of the list. It will be last to be considered/called.
    /// </summary>
    /// <param name="failStrategy"></param>
    /// <returns></returns>
    public RequestConfigAsync AddFailStrategy(IFailStrategyAsync failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    /// <summary>
    /// Adds a fail strategy to the top of the list. It will be first to be considered/called.
    /// </summary>
    /// <param name="failStrategy"></param>
    /// <returns></returns>
    public RequestConfigAsync AddFailStrategyToTop(IFailStrategyAsync failStrategy)
    {
        FailStrategies.Insert(0, failStrategy);
        return this;
    }

    /// <summary>
    /// Removes all failstrategies configured up until this point.
    /// </summary>
    /// <returns></returns>
    public RequestConfigAsync ResetFailStrategies()
    {
        FailStrategies.Clear();
        return this;
    }

    /// <summary>
    /// Removes all failstrategies that pass the given predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public RequestConfigAsync RemoveSelectedFailStrategies(Func<IFailStrategyAsync, bool> predicate)
    {
        FailStrategies = FailStrategies.Where(x => !predicate(x)).ToList();
        return this;
    }

    /// <summary>
    /// Sets the request saver. All requests will automatically be saved
    /// </summary>
    /// <param name="requestSaver"></param>
    /// <returns></returns>
    public RequestConfigAsync UsingRequestSaver(RequestSaverAsync requestSaver)
    {
        RequestSaverAsync = requestSaver;
        return this;
    }

    /// <summary>
    /// Sets the request loader.
    /// If it can find a match, then the request does not call the LLM, but instead loads it.
    /// </summary>
    /// <param name="requestLoader"></param>
    /// <returns></returns>
    public RequestConfigAsync UsingRequestLoader(RequestLoaderAsync requestLoader)
    {
        RequestLoaderAsync = requestLoader;
        return this;
    }
}
