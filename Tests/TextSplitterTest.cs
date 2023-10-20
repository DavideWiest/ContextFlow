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

        Assert.AreEqual(3, outputlist.Count);
        Assert.AreEqual("Test", outputlist[0]);
    }

    [Test]
    public void TestHierarchicalSplitter()
    {
        var splitter = new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), 5, new() {" "}, new());
        var outputlist = splitter.Split("Test test Test");

        Assert.AreEqual(2, outputlist.Count);
    }

    [Test]
    public void TestHierarchicalSplitterSplitting()
    {
        var splitter = new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), 12, new() { " " }, new());
        var outputlist = splitter.Split("Test test Test test test test");

        Assert.AreNotEqual("Test", outputlist[0]);
    }
}