using ContextFlow.Application.Result;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Storage;

internal static class LoaderUtil
{
    public static RequestResult ConvertToRequestResult(object responseObj)
    {
        return ((JObject)responseObj).ToObject<WritableRequestResult>()!.ToRequestResult();
    }
}
