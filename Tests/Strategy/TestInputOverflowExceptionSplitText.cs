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
                    .ActivateCheckNumTokensBeforeRequest(new CountCharsTokenizer())
                    .UsingFailStrategy(new InputOverflowStrategySplitText(new FunctionTextSplitter(x => x.Split("\n")), new SimpleSectionMerger(new() { { "BigAttachment: \n", "-" } }), "BigAttachment")))
                .UsingLLMConnection(new RepeatInputConnection());

    [Test]
    public void TestStrategy()
    {
        string content = new string('a', 50) + "\n" + new string('b', 50) + "\n"  + new string('c', 50);
        Attachment a = new Attachment("BigAttachment", content, false);

        var request = requestBuilder
            .UsingPrompt(new Prompt("action").UsingAttachment(a))
            .Build();

        var result = request.Complete();

        string expected = String.Join("-", content.Split("\n"));

        Assert.That(result.RawOutput, Is.EqualTo(expected));
    }
}