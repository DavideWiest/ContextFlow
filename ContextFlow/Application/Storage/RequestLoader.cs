using ContextFlow.Application.Request;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Storage;

/// <summary>
/// Loads requests if they exist.
/// Loaders and savers can be configured in the RequestConfig-class and will be used automatically, if they have been defined.
/// It is recommended to implement an option that defines if the LLM-configuration is supposed to be considered when looking for a match. However, since it isn't in this abstract class, it is not required.
/// </summary>
public abstract class RequestLoader
{
    /// <summary>
    /// The loader checks if a request exists (how, and in what scope is determined by the loader), and returns it if a match exists.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public RequestResult? LoadMatchIfExists(LLMRequest request)
    {
        bool matchExists = MatchExists(request);
        request.RequestConfig.Logger.Information("Trying to load the result of the current request - Match exists: {matchExists}", matchExists);
        return matchExists ? LoadMatch(request) : null;
    }
    /// <summary>
    /// Checks if a match exists for the given request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public abstract bool MatchExists(LLMRequest request);

    /// <summary>
    /// Loads match on the premise that it exists. If you load it manually, using LoadMatchIfExists instead is advised.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public abstract RequestResult LoadMatch(LLMRequest request);
}