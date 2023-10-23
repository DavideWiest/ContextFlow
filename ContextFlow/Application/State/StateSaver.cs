using ContextFlow.Application.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.State;

public abstract class RequestSaver
{
    public abstract void SaveState(LLMRequest request, RequestResult result);
}
