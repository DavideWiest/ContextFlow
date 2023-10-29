﻿using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Prompting;
using ContextFlow.Application.TextUtil;
using ContextFlow.Domain;
using ContextFlow.Application.Result;

namespace ContextFlow.Application.Strategy.Async;

public class InputOverflowStrategySplitTextAsync : FailStrategyAsync<InputOverflowException>
{
    private TextSplitter Splitter;
    private TextMerger Merger;
    private string SplitAttachmentName;

    public InputOverflowStrategySplitTextAsync(TextSplitter splitter, TextMerger merger, string splitAttachmentName)
    {
        Splitter = splitter;
        Merger = merger;
        SplitAttachmentName = splitAttachmentName;
    }

    public override async Task<RequestResult> ExecuteStrategy(LLMRequestAsync request, InputOverflowException e)
    {
        request.RequestConfig.Logger.Debug($"{GetType().Name} executing its strategy: Splitting attachment {SplitAttachmentName} and merging the outputs later on.");
        request.RequestConfig.Logger.Debug("However, InputOverflowExceptions may still occur if the rest of the prompt plus any one fragment of the attachment has too many tokens.");

        var attachment = request.Prompt.Attachments.FirstOrDefault(a => a.Name == SplitAttachmentName);
        if (attachment == null)
        {
            request.RequestConfig.Logger.Error($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
            throw new InvalidOperationException($"Attachment with the name {SplitAttachmentName} does not exist. Unable to split it up.");
        }

        var attachmentContentFragments = Splitter.Split(attachment.Content);

        request.RequestConfig.Logger.Debug("\n--- SPLIT ATTACHMENT ---\n" + string.Join("\n---\n", attachmentContentFragments) + "\n--- SPLIT ATTACHMENT ---\n");

        List<Task<RequestResult>> resultTasks = new();

        foreach (var fragment in attachmentContentFragments)
        {
            resultTasks.Add(new LLMRequestBuilder(request)
            .UsingPrompt(request.Prompt.UpsertingAttachment(new Attachment(SplitAttachmentName, fragment, true)))
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(new FailStrategyThrowExceptionAsync<InputOverflowException>()))
            .BuildAsync()
            .Complete());
        }

        RequestResult[] results = await CompleteAllFragmentRequests(request, attachmentContentFragments);

        return MergeResultsBack(results);
    }

    private async Task<RequestResult[]> CompleteAllFragmentRequests(LLMRequestAsync request, IEnumerable<string> attachmentContentFragments)
    {
        List<Task<RequestResult>> resultTasks = new();

        foreach (var fragment in attachmentContentFragments)
        {
            resultTasks.Add(new LLMRequestBuilder(request)
            .UsingPrompt(request.Prompt.UpsertingAttachment(new Attachment(SplitAttachmentName, fragment, true)))
            .UsingRequestConfig(request.RequestConfig.AddFailStrategyToTop(new FailStrategyThrowExceptionAsync<InputOverflowException>()))
            .BuildAsync()
            .Complete());
        }

        return await Task.WhenAll(resultTasks);
    }

    private RequestResult MergeResultsBack(IEnumerable<RequestResult> results)
    {
        return new RequestResult(Merger.Merge(results.Select(r => r.RawOutput).ToList()), results.ToList()[0].FinishReason);
    }

}