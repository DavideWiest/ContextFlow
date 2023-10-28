using ContextFlow.Infrastructure.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Fakes;

public class CountCharsTokenizer : LLMTokenizer
{
    public override int CountTokens(string input)
    {
        return input.Length;
    }
}
