using Microsoft.DeepDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public static class OverflowTextSplitter
{
    public static List<string> SplitText(string text, ITokenizer tokenizer)
    {
        return text.Split("\n\n")
    }
}
