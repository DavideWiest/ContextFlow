using ContextFlow.Application.Storage;

namespace ContextFlow.Application.Request;

public class RequestConfig : RequestConfigBase<RequestConfig>
{
    public List<IFailStrategy> FailStrategies { get; private set; } = new();
    public RequestLoader? RequestLoader { get; private set; }
    public RequestSaver? RequestSaver { get; private set; }

    public RequestConfig AddFailStrategy(IFailStrategy failStrategy)
    {
        FailStrategies.Add(failStrategy);
        return this;
    }

    public RequestConfig AddFailStrategyToTop(IFailStrategy failStrategy)
    {
        FailStrategies.Insert(0, failStrategy);
        return this;
    }

    public RequestConfig ResetFailStrategies()
    {
        FailStrategies.Clear();
        return this;
    }

    public RequestConfig RemoveSelectedFailStrategies(Func<IFailStrategy, bool> predicate)
    {
        FailStrategies = FailStrategies.Where(x => predicate(x)).ToList();
        return this;
    }

    public RequestConfig UsingRequestSaver(RequestSaver requestSaver)
    {
        RequestSaver = requestSaver;
        return this;
    }

    public RequestConfig UsingRequestLoader(RequestLoader requestLoader)
    {
        RequestLoader = requestLoader;
        return this;
    }


}
