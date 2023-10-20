using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class CFConverter<T>
{
    public abstract string FromObject(T obj, dynamic? data);
    public abstract T FromString(string obj, dynamic? data);
}

public class FunctionConverter<T> : CFConverter<T>
{
    private readonly Func<T, dynamic?, string> FromObjectFunc;
    private readonly Func<string, dynamic?, T> FromStringFunc;

    public FunctionConverter(Func<T, dynamic?, string> fromObjectFunc, Func<string, dynamic?, T> fromStringFunc)
    {
        FromObjectFunc = fromObjectFunc ?? throw new ArgumentNullException(nameof(fromObjectFunc));
        FromStringFunc = fromStringFunc ?? throw new ArgumentNullException(nameof(fromStringFunc));
    }

    public override string FromObject(T obj, dynamic? data)
    {
        return FromObjectFunc(obj, data);
    }

    public override T FromString(string obj, dynamic? data)
    {
        return FromStringFunc(obj, data);
    }
}