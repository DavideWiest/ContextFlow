using ContextFlow.Application;
using ContextFlow.Infrastructure.Providers.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

using ContextFlow.Application.TextUtil;

public class TextSplitterTest
{

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void TestFunctionSplitter()
    {
        var splitter = new FunctionTextSplitter(x => x.Split(" ").ToList());
        var outputlist = splitter.Split("Test test Test");

        Assert.That(outputlist.Count, Is.EqualTo(3));
        Assert.That(outputlist[0], Is.EqualTo("Test"));
    }

    [Test]
    public void TestHierarchicalSplitter()
    {
        var splitter = new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), 5, new() {" "}, new());
        var outputlist = splitter.Split("Test test test");

        Assert.That(outputlist.Count, Is.EqualTo(2));
    }

    [Test]
    public void TestHierarchicalSplitterSplitting()
    {
        var splitter = new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), 12, new() { " " }, new());
        var outputlist = splitter.Split("Test test test test test test");

        Assert.That(outputlist[0], Is.Not.EqualTo("Test"));
    }

    [Test]
    public void TestHierarchicalSplitterHierarchy()
    {
        var splitter = new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), 8, new() { "\n", " " }, new());
        var outputlist = splitter.Split("Test test test\nTest test test");

        Assert.That(outputlist[1], Is.EqualTo(outputlist[0]));
    }
}