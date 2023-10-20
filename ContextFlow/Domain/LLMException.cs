using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Domain;

public class LLMException : Exception
{
    public LLMException(string message) : base(message)
    {

    }
}
