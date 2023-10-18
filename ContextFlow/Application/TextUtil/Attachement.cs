using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public class Attachment
{
    public string Name { get; set; }
    public string Content { get; set; }
    public bool IsInline { get; set; } = false;

    public Attachment(string name, string content, bool isInline)
    {
        Name = name;
        Content = content;
        IsInline = isInline;
    }

    public string ToPlainText()
    {
        string sep = IsInline ? "\n" : "";
        return $"{Name}: {sep}{Content}";
    }

    public override string ToString()
    {
        return $"Attachment(Name=\"{Name}\", Content=\"{Content}\", IsInline=\"{IsInline}\")";
    }
}
