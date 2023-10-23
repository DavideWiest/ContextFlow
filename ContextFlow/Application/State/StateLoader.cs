using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.State;

public abstract class StateLoader
{
    public abstract RequestState LoadState();
}
