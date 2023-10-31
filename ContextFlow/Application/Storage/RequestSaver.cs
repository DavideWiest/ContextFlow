using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage;

/// <summary>
/// Saves requests in the configured context of the implemented RequestSaver-class.
/// Loaders and savers can be configured in the RequestConfig-class and will be used automatically, if they have been defined.
/// </summary>
public abstract class RequestSaver
{
    /// <summary>
    /// Extracts the data that represents the key of the request (Prompt and LLMConfig recommended), and saves it.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="result">The request's result</param>
    public abstract void SaveRequest(LLMRequest request, RequestResult result);
}
