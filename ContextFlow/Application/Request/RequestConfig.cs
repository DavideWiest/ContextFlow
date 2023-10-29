using ContextFlow.Application.Storage;

namespace ContextFlow.Application.Request;

/// <summary>
/// Configuration of a request. This configuration applies only to the LLMRequest/LLMRequestAsync class, not any others.
/// </summary>
public class RequestConfig : RequestConfigBase<RequestConfig>
{
    /// <summary>
    /// A list of failstrategies, which will be used to catch and handle exceptions.
    /// </summary>
    public List<IFailStrategy> FailStrategies { get; private set; } = new();
    public RequestLoader? RequestLoader { get; private set; }
    public RequestSaver? RequestSaver { get; private set; }

    /// <summary>
    /// Adds a fail strategy to the bottom of the list. It will be last to be considered/called.
    /// </summary>
    /// <param name="failStrategy"></param>
    /// <returns></returns>
    public RequestConfig AddFailStrategy(IFailStrategy failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    /// <summary>
    /// Adds a fail strategy to the top of the list. It will be first to be considered/called.
    /// </summary>
    /// <param name="failStrategy"></param>
    /// <returns></returns>
    public RequestConfig AddFailStrategyToTop(IFailStrategy failStrategy)
    {
        FailStrategies.Insert(0, failStrategy);
        return this;
    }

    /// <summary>
    /// Removes all failstrategies configured up until this point.
    /// </summary>
    /// <returns></returns>
    public RequestConfig ResetFailStrategies()
    {
        FailStrategies.Clear();
        return this;
    }

    /// <summary>
    /// Removes all failstrategies that pass the given predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public RequestConfig RemoveSelectedFailStrategies(Func<IFailStrategy, bool> predicate)
    {
        FailStrategies = FailStrategies.Where(x => !predicate(x)).ToList();
        return this;
    }

    /// <summary>
    /// Sets the request saver. All requests will automatically be saved
    /// </summary>
    /// <param name="requestSaver"></param>
    /// <returns></returns>
    public RequestConfig UsingRequestSaver(RequestSaver requestSaver)
    {
        RequestSaver = requestSaver;
        return this;
    }

    /// <summary>
    /// Sets the request loader.
    /// If it can find a match, then the request does not call the LLM, but instead loads it.
    /// </summary>
    /// <param name="requestLoader"></param>
    /// <returns></returns>
    public RequestConfig UsingRequestLoader(RequestLoader requestLoader)
    {
        RequestLoader = requestLoader;
        return this;
    }


}
