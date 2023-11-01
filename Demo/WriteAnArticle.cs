using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Storage.Json;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers.OpenAI;

namespace Demo;

public static class WriteAnArticle
{
    public static string Write(string topic)
    {
        // define the prompts that will be used below
        var outlineprompt = new Prompt($"Write an outline for an article about {topic}.")
            .UsingOutputDescription("A simple unordered list consisting of headings");

        var writeprompt = new Prompt($"Write a paragraph of appropriate length about following topic. Your text will be a part of an article about {topic}.");

        // create a connection to the OpenAI API
        var con = new OpenAIChatConnection(); // With api-key: new OpenAIChatConnection("<api-key>");

        // Create a llm-configuration instance and specify the model name, maximum total tokens, and maximum input tokens
        var llmconf = new LLMConfig("gpt-3.5-turbo-16k", 1024, 512)
            .UsingSystemMessage("You are a creative writer.");

        // instantiate a request configuration object and define a json request-saver and -loader
        var requestconf = new RequestConfig()
            .UsingRequestSaver(new JsonRequestSaver("articles.json"))
            .UsingRequestLoader(new JsonRequestLoader("articles.json"));

        // create a builder and set the values that won't change in future prompts
        var builder = new LLMRequestBuilder()
            .UsingLLMConnection(con)
            .UsingLLMConfig(llmconf)
            .UsingRequestConfig(requestconf);

        // define the prompt of the request and build it
        IEnumerable<string> outline = builder
            .UsingPrompt(outlineprompt)
            .Build()
            // executing the request
            .Complete()
            // parsing the output into an IEnumerable of strings, and converting the ParsedOutput of the ParsedResultRequest to a list.
            .Actions.Parse(
                // extract the unordered lists items
                result => result.RawOutput.Split("\n")
                .Where(line => line.StartsWith("- "))
                .Select(line => line[2..])
            ).ParsedOutput;

        // join each heading with its content which is generated from another request
        IEnumerable<string> articleSegments = outline.Select(
            section => 
            "## " + section +
            builder
            // use a prompt and add an inline attachment to it, which contains the respective topic
            .UsingPrompt(writeprompt.UsingAttachment(new Attachment("Topic", section, true)))
            .Build()
            .Complete()
            .RawOutput
        );

        // merge the sections into an article and return it
        string article = String.Join("\n\n", articleSegments);
        return article;
    }
}
