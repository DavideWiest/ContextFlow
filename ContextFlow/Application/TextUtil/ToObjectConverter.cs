using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class ToObjectConverter<T>
{
    public abstract T Convert(string obj);
}

public class ToObjectFunctionConverter<T> : ToObjectConverter<T>
{
    private readonly Func<string, T> ConverterFunc;

    public ToObjectFunctionConverter(Func<string, T> converterFunc)
    {
        ConverterFunc = converterFunc;
    }

    public override T Convert(string obj)
    {
        return ConverterFunc(obj);
    }
}