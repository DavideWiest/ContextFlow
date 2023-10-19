using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class CFConverter<T>
{
    public abstract string ToString(T obj);
    public abstract T FromString(string obj);
}
