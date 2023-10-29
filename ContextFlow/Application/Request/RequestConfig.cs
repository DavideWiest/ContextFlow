using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Storage;
using ContextFlow.Application.Strategy;

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

    public override string ToString()
    {
        string failStrategies = string.Join(", ", FailStrategies.Select(fs => fs.GetType().Name));
        string requestLoader = RequestLoader != null ? RequestLoader.GetType().Name : "null";
        string requestSaver = RequestSaver != null ? RequestSaver.GetType().Name : "null";
        string tokenizer = Tokenizer != null ? Tokenizer.GetType().Name : "null";

        return $"RequestConfig(FailStrategies=[{failStrategies}], Logger={Logger.GetType().Name}, RequestLoader={requestLoader}, RequestSaver={requestSaver}, ValidateNumInputTokensBeforeRequest={ValidateNumInputTokensBeforeRequest}, ThrowExceptionOnOutputOverflow={ThrowExceptionOnOutputOverflow}, Tokenizer={tokenizer})";
    }
}
