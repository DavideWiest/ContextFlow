using ContextFlow.Application.Prompting;
using ContextFlow.Application.Result;
using ContextFlow.Application.Request;
using ContextFlow.Application.Request.Async;
using ContextFlow.Application.Storage.Async.Json;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers.OpenAI.Async;
using static System.Collections.Specialized.BitVector32;

namespace Demo;

public class WriteAnArticleAsync
{
    public static async Task<string> Write(string about)
    {
        // define the prompts that will be used below
        var outlineprompt = new Prompt($"Write an outline for an article about {about}.")
            .UsingOutputDescription("A simple unordered list consisting of headings");

        var writeprompt = new Prompt($"Write a paragraph of appropriate length about following topic. Your text will be a part of an article about {about}.");

        // create a connection to the OpenAI API
        var con = new OpenAIChatConnectionAsync(); // With api-key: new OpenAIChatConnectionAsync("<api-key>");

        // Create a llm-configuration instance and specify the model name, maximum total tokens, and maximum input tokens
        var llmconf = new LLMConfig("gpt-3.5-turbo-16k", 1024, 512)
            .UsingSystemMessage("You are a creative writer.");

        // instantiate a request configuration object and define a json request-saver and -loader
        var requestconf = new RequestConfigAsync()
            .UsingRequestSaver(new JsonRequestSaverAsync("articles.json"))
            .UsingRequestLoader(new JsonRequestLoaderAsync("articles.json"));

        // create a builder and set the values that won't change in future prompts
        var builder = new LLMRequestBuilder()
            .UsingLLMConnection(con)
            .UsingLLMConfig(llmconf)
            .UsingRequestConfig(requestconf);

        // define the prompt of the request and build it
        RequestResult outlineResult = await builder
            .UsingPrompt(outlineprompt)
            .BuildAsync()
            // executing the request
            .Complete();

        // parsing the output into an IEnumerable of strings, and converting the ParsedOutput of the ParsedResultRequest to a list.
        ParsedRequestResult<List<string>> outlineParsedResult = outlineResult
        .Actions.Parse(
            // extract the unordered lists items
            result => result.RawOutput.Split("\n")
            .Where(line => line.StartsWith("- "))
            .Select(line => line[2..])
            .ToList()
        );

        // join each heading with its content which is generated from another request
        var contentResults = await outlineParsedResult.AsyncActions.ThenBranching(
            outline => BuildContentRequest(builder, writeprompt, outline.ParsedOutput)
        );

        // interlace the headings and contents
        var articleSegments = contentResults.Select(
            (result, i) => "## " + outlineParsedResult.ParsedOutput[i] + result.RawOutput
        );

        // merge the sections into an article and return it
        string article = String.Join("\n\n", articleSegments);
        return article;
    }

    private static IEnumerable<LLMRequestAsync> BuildContentRequest(LLMRequestBuilder builder, Prompt prompt, IEnumerable<string> outline)
    {
        var requests = outline.Select(
            section =>
            builder
            // use a prompt and add an inline attachment to it, which contains the respective topic
            .UsingPrompt(prompt.UsingAttachment(new Attachment("Topic", section, true)))
            .BuildAsync()
        );

        return requests;
    }
}
