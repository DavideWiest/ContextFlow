using ContextFlow.Application.Result;
using Newtonsoft.Json.Linq;

namespace ContextFlow.Application.Storage.Json;

internal static class LoaderUtil
{
    public static RequestResult ConvertToRequestResult(object responseObj)
    {
        return ((JObject)responseObj).ToObject<WritableRequestResult>()!.ToRequestResult();
    }
}
