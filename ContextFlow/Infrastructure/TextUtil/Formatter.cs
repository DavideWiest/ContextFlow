using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Infrastructure.Formatter;

public abstract class Formatter
{
    public abstract string Format(string str, Dictionary<string, object> data);
    public abstract List<string> GetUndefinedPlaceholderValues(string str, Dictionary<string, object> formatParams);
}
