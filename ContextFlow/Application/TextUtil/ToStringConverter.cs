using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public abstract class ToStringConverter<T>
{
    public abstract string Convert(T obj);
}

public class ToStringFunctionConverter<T> : ToStringConverter<T>
{
    private readonly Func<T, string> ConverterFunc;

    public ToStringFunctionConverter(Func<T, string> converterFunc)
    {
        ConverterFunc = converterFunc;
    }

    public override string Convert(T obj)
    {
        return ConverterFunc(obj);
    }
}