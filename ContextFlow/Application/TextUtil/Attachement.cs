using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public class Attachement
{
    public string Name { get; set; }
    public string Content { get; set; }
    public bool IsInline { get; set; } = false;

    public Attachement(string name, string content, bool isInline)
    {
        Name = name;
        Content = content;
        IsInline = isInline;
    }
}
