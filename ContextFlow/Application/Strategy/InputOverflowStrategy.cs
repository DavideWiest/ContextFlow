using ContextFlow.Application.Request;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Strategy;

public abstract class InputOverflowStrategy : FailStrategy<InputOverflowException>
{
    public abstract override RequestResult ExecuteStrategy(LLMRequest request, InputOverflowException e);
}

public class InputOverflowStrategySplitText : InputOverflowStrategy
{
    private TextSplitter Splitter;
    private TextMerger Merger;
    private string SplitAttachmentName;

    public InputOverflowStrategySplitText(TextSplitter splitter, TextMerger merger, string splitAttachmentName)
    {
        Splitter = splitter;
        Merger = merger;
        SplitAttachmentName = splitAttachmentName;
    }

    public override RequestResult ExecuteStrategy(LLMRequest request, InputOverflowException e)
    {
        request.RequestConfig.Logger.Debug("Info: InputOverflowExceptions may still occur if the rest of the prompt plus any one fragment of the attachment has too many tokens.");

        var attachment = request.Prompt.Attachments.FirstOrDefault(a => a.Name == SplitAttachmentName);
        if (attachment == null)
        {
            request.RequestConfig.Logger.Error("Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.", SplitAttachmentName);
            throw new InvalidOperationException($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
        }

        var attachmentContentFragments = Splitter.Split(attachment.Content);

        request.RequestConfig.Logger.Debug("\n--- SPLIT ATTACHMENT ---\n" + "{splitattachment}" + "\n--- SPLIT ATTACHMENT ---\n", string.Join("\n---\n", attachmentContentFragments));

        List<RequestResult> results = CompleteAllFragmentRequests(request, attachmentContentFragments);

        return MergeResultsBack(results);
    }

    private List<RequestResult> CompleteAllFragmentRequests(LLMRequest request, IEnumerable<string> attachmentContentFragments)
    {
        List<RequestResult> results = new();
        foreach (var fragment in attachmentContentFragments)
        {
            results.Add(new LLMRequestBuilder(request)
            .UsingPrompt(request.Prompt.UpsertingAttachment(new Attachment(SplitAttachmentName, fragment, true)))
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(new FailStrategyThrowException<InputOverflowException>()))
            .Build()
            .Complete());
        }
        return results;
    }

    private RequestResult MergeResultsBack(List<RequestResult> results)
    {
        return new RequestResult(Merger.Merge(results.Select(r => r.RawOutput).ToList()), results[0].FinishReason);
    }
}

public class InputOverflowStrategyThrowException : InputOverflowStrategy
{
    public override RequestResult ExecuteStrategy(LLMRequest request, InputOverflowException e)
    {
        throw e;
    }
}