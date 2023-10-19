using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public class DefaultConverter: CFConverter
{
    private bool NewLineSpacing = true;
    public DefaultConverter(bool newLineSpacing) {
        NewLineSpacing = newLineSpacing;
    }
    public override string FromDynamic(dynamic obj)
    {
        string nlSpacing = NewLineSpacing ? "\n" : "";
        StringBuilder sb = new StringBuilder();
        if (obj is IDictionary)
        {
            foreach (var kv in obj)
            {
                sb.Append(nlSpacing);
                sb.Append("# " + kv.Key.ToString());
                sb.Append(nlSpacing);
                sb.Append(FromDynamic(kv.Value));
                sb.Append(nlSpacing);
            }
        } else if (obj is IEnumerable)
        {
            foreach (var elem in obj) {
                sb.Append("- " + (string)elem.Key);
                sb.Append("\n");
            }
        } else
        {
            sb.Append(obj.ToString());
        }
        
        return sb.ToString();
    }

    public override dynamic ToDynamic(string str)
    {
        var lines = str.Split('\n');
        var stack = new Stack<dynamic>();
        var current = new ExpandoObject();
        stack.Push(current);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            // Determine heading level
            var headingLevel = 0;
            while (headingLevel < trimmedLine.Length && trimmedLine[headingLevel] == '#')
                headingLevel++;

            // Extract the text after the heading
            var text = trimmedLine.Substring(headingLevel).Trim();

            // Create the new object for this line
            var newObject = new ExpandoObject();
            var newDictionary = (IDictionary<string, object>)newObject;
            newDictionary["text"] = text;

            if (headingLevel == 0)
            {
                // No heading, treat it as a plain string
                ((List<dynamic>)((stack.Peek() as IDictionary<string, object>)["text"])).Add(text);
            }
            else
            {
                // Find the parent object based on heading level
                while (stack.Count > headingLevel)
                {
                    stack.Pop();
                }

                // Add the new object to the parent
                if (stack.Peek() is List<dynamic> parentList)
                {
                    parentList.Add(newObject);
                }
                else
                {
                    var parentDictionary = (IDictionary<string, object>)stack.Peek();
                    parentDictionary[text] = newObject;
                }
                stack.Push(newObject);
            }
        }

        return current;
    }

}
