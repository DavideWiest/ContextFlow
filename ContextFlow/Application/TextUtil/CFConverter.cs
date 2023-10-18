using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class CFConverter
{
    public abstract string FromDynamic(dynamic obj);
    public abstract dynamic ToDynamic(string obj);
}
