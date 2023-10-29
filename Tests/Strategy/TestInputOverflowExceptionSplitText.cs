using ContextFlow.Application.Request;
using ContextFlow.Domain;
using ContextFlow.Application.Prompting;
using System;
using System.Collections.Generic;
using ContextFlow.Application.Strategy;
using ContextFlow.Application.TextUtil;
using Tests.Fakes;

namespace Tests.Strategy;

public class TestInputOverflowExceptionSplitText
{

    LLMRequestBuilder requestBuilder = new LLMRequestBuilder()
                .UsingLLMConfig(new LLMConfig("model").UsingMaxInputTokens(50))
                .UsingRequestConfig(new RequestConfig()
                    .AddFailStrategy(new InputOverflowStrategySplitText(new FunctionTextSplitter(x => x.Split("\n")), new SimpleSectionMerger(new() { { "BigAttachment: \n", "-" } }), "BigAttachment"))
                    .ActivateCheckNumTokensBeforeRequest(new CountCharsTokenizer()))
                .UsingLLMConnection(new RepeatInputConnection());

    [Test]
    public void TestStrategy()
    {
        string content = new string('a', 25) + "\n" + new string('b', 25) + "\n"  + new string('c', 25);
        Attachment a = new Attachment("BigAttachment", content, false);

        var request = requestBuilder
            .UsingPrompt(new Prompt("action").UsingAttachment(a))
            .Build();

        var result = request.Complete();

        string expected = "BigAttachment: \n" + String.Join("-", content.Split("\n"));

        Console.WriteLine(result.RawOutput);
        Assert.That(result.RawOutput, Is.EqualTo(expected));
    }
}