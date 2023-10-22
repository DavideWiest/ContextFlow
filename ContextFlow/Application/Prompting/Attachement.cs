using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Prompting;

public class Attachment
{
    public string? Name { get; set; } = null;
    public string Content { get; set; }
    public bool IsInline { get; set; } = false;

    public Attachment(string? name, string content, bool isInline)
    {
        Name = name;
        Content = content;
        IsInline = isInline;
    }

    public Attachment(string content, bool isInline)
    {
        Content = content;
        IsInline = isInline;
    }

    public string ToPlainText()
    {
        string sep = !IsInline ? "\n" : "";
        string namestr = Name != null && Name != string.Empty && Name != "" ? Name + ": " : string.Empty;
        return $"{namestr}{sep}{Content}\n";
    }

    public override string ToString()
    {
        return $"Attachment(Name=\"{Name}\", Content=\"{Content}\", IsInline=\"{IsInline}\")";
    }

    /// <summary>
    /// Shallow copy using ICloneable
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Attachment Clone()
    {
        return (Attachment)MemberwiseClone();
    }
}
