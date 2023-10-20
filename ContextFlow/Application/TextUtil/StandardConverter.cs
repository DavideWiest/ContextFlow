﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

/// <summary>
/// A basic converter for dynamic content. 
/// Important: When converting to string, it will handle all properties of an object, which can lead to a lot of clutter.
/// It converts Dictionaries and Enumerables to a unordered list, if their values are strings. 
/// Objects are recursively divided into a section for each property, represented by markdown-headings.
/// </summary>
/// <typeparam name="T">The given type that the converter handles - Must be instantiatable with 0 parameters</typeparam>
public class StandardConverter<T> : CFConverter<T> where T : new()
{
    public override string FromObject(T obj, dynamic? additionalData)
    {
        int depth = 0;
        if (additionalData == null)
        {
            depth = additionalData;
        }

        if (obj == null)
        {
            return "null";
        }

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();

        StringBuilder result = new StringBuilder();

        foreach (var property in properties)
        {
            result.Append(new string('#', depth) + " ")
                  .Append(property.Name)
                  .Append("\n");

            object? value = property.GetValue(obj);

            if (value == null) { continue; }

            if (value is object)
            {
                if (value is Dictionary<object, string>)
                {
                    foreach(var kvp in ((Dictionary<object, string>)value))
                    {
                        result.Append("- ")
                            .Append(kvp.Key.ToString())
                            .Append(": ")
                            .Append(kvp.Value)
                            .Append("\n");
                    }
                } else if (value is IEnumerable) {
                    if (((IEnumerable)value).AsQueryable().GetEnumerator().Current is string)
                    {
                        foreach(object v in  ((IEnumerable)value))
                        {
                            result.Append("- ")
                                .Append(v)
                                .Append("\n");
                        }
                    } else
                    {
                        result.Append(new StandardConverter<object>().FromObject(value, depth + 1));
                    }
                } else 
                {
                    result.Append(new StandardConverter<object>().FromObject(value, depth + 1));
                }
            }
            else
            {
                result.Append(value.ToString());
            }
        }

        return result.ToString();
    }

    public override T FromString(string input, dynamic? additionalData)
    {
        T obj = new T();
        Type type = obj.GetType();
        string[] lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            int depth = line.Count(c => c == '#');
            string propertyName = line.TrimStart('#');

            PropertyInfo? property = type.GetProperty(propertyName);

            if (property != null)
            {
                string valueString = lines[Array.IndexOf(lines, line) + 1];
                object value = Convert.ChangeType(valueString, property.PropertyType);
                property.SetValue(obj, value);
            }
        }

        return obj;
    }
}