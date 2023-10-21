using ContextFlow.Application.Request;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;

namespace ContextFlow.Application;

public abstract class OverflowStrategy : FailStrategy<TokenOverflowException>
{
    public abstract override RequestResult ExecuteStrategy(LLMRequest request, TokenOverflowException e);
}

public class OverflowStrategySplitText : OverflowStrategy
{
    private TextSplitter Splitter;
    private TextMerger Merger;
    private string SplitAttachmentName;

    public OverflowStrategySplitText(TextSplitter splitter, TextMerger merger, string splitAttachmentName)
    {
        Splitter = splitter;
        Merger = merger;
        SplitAttachmentName = splitAttachmentName;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, TokenOverflowException e)
    {
        var attachment = request.Prompt.Attachments.FirstOrDefault(a => a.Name == SplitAttachmentName);
        if (attachment == null)
        {
            throw new InvalidOperationException($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
        }
        var attachmentContentFragments = Splitter.Split(attachment.Content);
        
        List<RequestResult> results = new List<RequestResult>();
        foreach ( var fragment in attachmentContentFragments )
        {
            results.Add(new LLMRequestBuilder(request)
            .UsingPrompt(request.Prompt.UsingAttachment(SplitAttachmentName, fragment))
            .Build()
            .Complete());
        }
        return new RequestResult(Merger.Merge(results.Select(r => r.RawOutput).ToList()), results[0].FinishReason);
    }
}

public class OverflowStrategyThrowException : OverflowStrategy
{
    public override RequestResult ExecuteStrategy(LLMRequest request, TokenOverflowException e)
    {
        throw e;
    }
}