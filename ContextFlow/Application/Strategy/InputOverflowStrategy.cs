﻿using ContextFlow.Application.Request;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;

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
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy: Splitting attachment {SplitAttachmentName} and merging the outputs later on.");

        var attachment = request.Prompt.Attachments.FirstOrDefault(a => a.Name == SplitAttachmentName);
        if (attachment == null)
        {
            request.RequestConfig.Logger.Error($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
            throw new InvalidOperationException($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
        }

        var attachmentContentFragments = Splitter.Split(attachment.Content);

        request.RequestConfig.Logger.Debug("\n--- SPLIT ATTACHMENT ---\n" + string.Join("\n---\n", attachmentContentFragments) + "\n--- SPLIT ATTACHMENT ---\n");

        List<RequestResult> results = new List<RequestResult>();
        foreach (var fragment in attachmentContentFragments)
        {
            results.Add(new LLMRequestBuilder(request)
            .UsingPrompt(request.Prompt.UsingAttachment(SplitAttachmentName, fragment))
            .Build()
            .Complete());
        }
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