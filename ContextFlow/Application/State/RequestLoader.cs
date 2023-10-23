using ContextFlow.Application.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.State;

public abstract class RequestLoader
{
    public RequestResult? LoadMatchIfExists(LLMRequest request)
    {
        return MatchExists(request) ? LoadMatch(request) : null;
    }
    public abstract bool MatchExists(LLMRequest request);
    public abstract RequestResult LoadMatch(LLMRequest request);
}
