using ContextFlow.Application.Storage.Async;
using ContextFlow.Application.Strategy.Async;

namespace ContextFlow.Application.Request.Async;

public class RequestConfigAsync : RequestConfigBase
{
    public List<IFailStrategyAsync> FailStrategies { get; private set; } = new();
    public RequestLoaderAsync? RequestLoaderAsync { get; private set; }
    public RequestSaverAsync? RequestSaverAsync { get; private set; }

    public RequestConfigAsync UsingFailStrategy(IFailStrategyAsync failStrategy)
    {
        FailStrategies.Add(failStrategy);
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

    public override string ToString()
    {
        string failStrategies = string.Join(", ", FailStrategies.Select(fs => fs.GetType().Name));
        string requestLoader = RequestLoaderAsync != null ? RequestLoaderAsync.GetType().Name : "null";
        string requestSaver = RequestSaverAsync != null ? RequestSaverAsync.GetType().Name : "null";
        string tokenizer = Tokenizer != null ? Tokenizer.GetType().Name : "null";

        return $"RequestConfigAsync(FailStrategies=[{failStrategies}], Logger={Logger.GetType().Name}, RequestLoader={requestLoader}, RequestSaver={requestSaver}, ValidateNumInputTokensBeforeRequest={ValidateNumInputTokensBeforeRequest}, RaiseExceptionOnOutputOverflow={RaiseExceptionOnOutputOverflow}, Tokenizer={tokenizer})";
    }
}
