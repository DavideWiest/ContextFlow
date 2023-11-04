
using ContextFlow.Infrastructure.Providers.OpenAI;
using Demo;

Console.WriteLine(WriteAnArticle.Write("the history of India"));
//Console.WriteLine(await WriteAnArticleAsync.Write("the history of India"));
//Console.WriteLine(RecursiveSummarizer.Summarize("<book or long article>", new OpenAITokenizer("gpt-3.5-turbo"), 1024));
//Console.WriteLine(CompleteAssignment.CompleteAssignmentFromSource("<source>"));