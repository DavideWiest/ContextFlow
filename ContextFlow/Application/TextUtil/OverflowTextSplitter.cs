using ContextFlow.Domain;
using Microsoft.DeepDev;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public static class OverflowTextSplitter
{
    public static List<string> SplitText(string text, LLMTokenizer tokenizer)
    {
        return text.Split(new string[] { "\n\n" }, StringSplitOptions.None).ToList();
    }
}
