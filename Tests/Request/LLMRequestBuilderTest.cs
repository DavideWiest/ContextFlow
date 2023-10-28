using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers;
using Tests.Fakes;
using Tests.Util;

namespace Tests.Request;

public class LLMRequestBuilderTest
{
    LLMConnection con = new SayHiConnection();
    LLMConnectionAsync conAsync = new SayHiConnectionAsync();
    RequestConfig requestConfig = new();
    RequestConfigAsync requestConfigAsync = new();
    LLMConfig llmconf = new("gpt-3.5-turbo");
    Prompt prompt = new("Say hi");

    LLMRequestBuilder builder = new LLMRequestBuilder();

    [SetUp]
    public void Setup()
    {
        builder
            .UsingPrompt(prompt)
            .UsingLLMConfig(llmconf);
    }

    [Test]
    public void TestBuilder()
    {
        var requestManual = new LLMRequest(prompt, llmconf, con, requestConfig);
        var requestFromBuilder = builder
            .UsingLLMConnection(con)
            .UsingRequestConfig(requestConfig)
            .Build();

        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.LLMConfig, requestFromBuilder.LLMConfig, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.RequestConfig, requestFromBuilder.RequestConfig, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.Prompt, requestFromBuilder.Prompt, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.LLMConnection, requestFromBuilder.LLMConnection, new string[0]));
    }

    [Test]
    public void TestBuilderAsync()
    {
        var requestManual = new LLMRequestAsync(prompt, llmconf, conAsync, requestConfigAsync);
        var requestFromBuilder = builder
            .UsingLLMConnection(conAsync)
            .UsingRequestConfig(requestConfigAsync)
            .BuildAsync();

        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.LLMConfig, requestFromBuilder.LLMConfig, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.RequestConfig, requestFromBuilder.RequestConfig, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.Prompt, requestFromBuilder.Prompt, new string[0]));
        Assert.That(InstanceComparer.PublicInstancePropertiesEqual(requestManual.LLMConnection, requestFromBuilder.LLMConnection, new string[0]));
    }

    [Test]
    public void TestBuilderFailingWithoutPrompt()
    {
        try
        {
            BuilderWithoutItem("prompt").Build();
            Assert.Fail();
        } catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void TestBuilderFailingWithoutLLMConfig()
    {
        try
        {
            BuilderWithoutItem("llmConfig").Build();
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void TestBuilderFailingWithoutConnection()
    {
        try
        {
            BuilderWithoutItem("connection").Build();
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void TestBuilderFailingWithoutConnectionAsync()
    {
        try
        {
            BuilderWithoutItem("connectionAsync").BuildAsync();
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void TestBuilderFailingWithoutRequestConfig()
    {
        try
        {
            BuilderWithoutItem("requestConfig").Build();
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void TestBuilderFailingWithoutRequestConfigAsync()
    {
        try
        {
            BuilderWithoutItem("requestConfigAsync").BuildAsync();
            Assert.Fail();
        }
        catch (InvalidOperationException)
        {
            Assert.Pass();
        }
    }



    public LLMRequestBuilder BuilderWithoutItem(string itemToIgnore)
    {
        if (itemToIgnore != "prompt")
            builder.UsingPrompt(prompt);
        if (itemToIgnore != "llmConfig")
            builder.UsingLLMConfig(llmconf);
        if (itemToIgnore != "connection")
            builder.UsingLLMConnection(con);
        if (itemToIgnore != "connectioAsync")
            builder.UsingLLMConnection(conAsync);
        if (itemToIgnore != "requestConfig")
            builder.UsingRequestConfig(requestConfig);
        if (itemToIgnore != "requestConfigAsync")
            builder.UsingRequestConfig(requestConfigAsync);

        return builder;
    }

}
