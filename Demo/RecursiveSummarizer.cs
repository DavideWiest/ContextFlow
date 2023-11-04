using ContextFlow.Application.Templates;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using ContextFlow.Infrastructure.Providers.OpenAI;
using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo;

public static class RecursiveSummarizer
{
    public static (string Summary, int Depth) Summarize(string input, LLMTokenizer tokenizer, int summaryTokenLen, int nSubSummariesPerNextSummary)
    {
        // split the inputted text
        var inputsSplit = new HierarchichalTextSplitter(tokenizer, summaryTokenLen, HierarchichalTextSplitter.MarkdownBasedHierarchy, HierarchichalTextSplitter.MarkdownBasedAddToBeginnings)
            .Split(input);

        var summaries = inputsSplit;
        int depth = 0;

        // while there is more than one summary or a summary is longer than the maximum number of tokens
        while (summaries.Count > 1 || tokenizer.CountTokens(summaries[0]) > summaryTokenLen)
        {
            depth++;
            // group by nSubSummariesPerNextSummary and merge into one string per group
            var summaryBlocks = GroupByCount(summaries, nSubSummariesPerNextSummary).Select(ls => String.Join("\n", ls));
            
            // summarize strings of grouped summaries into the next summary
            summaries = SummarizeInner(summaryBlocks, summaryTokenLen);
        }

        return (String.Join("\n\n", summaries), depth);
    }

    private static List<List<string>> GroupByCount(List<string> inputs, int groupsize)
    {
        var result = new List<List<string>>();
        for (int i = 0; i < inputs.Count; i += groupsize)
        {
            result.Add(inputs.Skip(i).Take(groupsize).ToList());
        }
        return result;
    }

    private static List<string> SummarizeInner(IEnumerable<string> inputs, int summaryTokenLen)
    {
        var con = new OpenAIChatConnection();
        var result = new List<string>();

        foreach (var input in inputs)
        {
            int availableTokenSpace = summaryTokenLen;
            double tokenToWordRatio = 3.5;
            double marginOfSafetyMul = 0.8;
            int availableWords = (int)Math.Floor(availableTokenSpace / tokenToWordRatio * marginOfSafetyMul);

            var targetLength = $"Below {availableWords} words";
            result.Add(new SummarizeTemplate(input, targetLength).GetLLMRequest(con, "gpt-3.5-turbo").Complete().RawOutput);
        }
        return result;
    }
    
}
