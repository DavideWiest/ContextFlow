using ContextFlow.Application.Prompting;
using ContextFlow.Application.Request;
using ContextFlow.Application.Storage.Json;
using ContextFlow.Domain;
using ContextFlow.Infrastructure.Providers.OpenAI;
using ContextFlow.Application.TextUtil;
using SmartFormat.Core.Extensions;
using System.Linq;

namespace Demo;

public static class CompleteAssignment
{
    private static readonly int MaxTotalTokens = 2500;
    private static readonly int MaxInputTokens = 1500;

    private static readonly Prompt ExtractAssignmentsPrompt =
        new Prompt("Extract the assignments given in this document. If there are none, say nothing.")
            .UsingOutputDescription("An unordered list consisting of the assignments");

    private static readonly Prompt FindRelevantTextsPrompt =
        new Prompt("You are given a list of text headings, and an assignment. Tell me which texts are relevant to the assingment")
            .UsingOutputDescription("An unordered list consisting of the relevant texts");

    private static readonly Prompt FindHeadingPrompt =
        new Prompt("Find a specific heading for this text")
               .UsingOutputDescription("The heading only");

    private static readonly Prompt WriteSolutionPrompt =
        new Prompt("Complete the given assignment by using and referencing the given texts")
            .UsingOutputDescription("A format that is appropriate fór the solution");

    public static Dictionary<string, string> CompleteAssignmentFromSource(string source)
    {

        // create a connection to the OpenAI API
        var con = new OpenAIChatConnection(); // With api-key: new OpenAIChatConnection("<api-key>");

        // Create a llm-configuration instance and specify the model name, maximum total tokens, and maximum input tokens
        var llmconf = new LLMConfig("gpt-3.5-turbo-16k", MaxTotalTokens, MaxInputTokens)
            .UsingSystemMessage("You are a creative writer.");

        // instantiate a request configuration object and define a json request-saver and -loader
        var requestconf = new RequestConfig()
            .UsingRequestSaver(new JsonRequestSaver("assignments.json"))
            .UsingRequestLoader(new JsonRequestLoader("assignments.json"));

        // create a builder and set the values that won't change in future prompts
        var builder = new LLMRequestBuilder()
            .UsingLLMConnection(con)
            .UsingLLMConfig(llmconf)
            .UsingRequestConfig(requestconf);

        // split up the source in case it is too large
        List<string> splitSource = SplitSource(source);

        // define the variables to be extracted from the source
        List<string> assignments = new();
        Dictionary<string, string> headingContentMap = new();

        foreach (string subsource in splitSource)
        {
            // extract assignments if they exist
            assignments.AddRange(ExtractAssignments(builder, subsource));

            // find a heading for this part of the text
            headingContentMap[FindHeading(builder, subsource)] = subsource;
        }
        
        // map the assignments to relevent texts, which will later be passed into the final prompt, 
        Dictionary<string, List<string>> assignmentToHeadingMap = new();

        foreach (string assignment in assignments)
        {
            assignmentToHeadingMap[assignment] = ExtractRelevantTextsByHeading(builder, assignment, headingContentMap.Keys);
        }

        // let the LLM write the solutions to each assignment and return it
        var assignmentSolutionMap = new Dictionary<string, string>();
         
        // pass the relevant texts with headings into the method that lets the LLm complete each assignment
        foreach (var kvp in assignmentToHeadingMap)
        {
            assignmentSolutionMap[kvp.Key] = WriteAssignmentSolution(
                builder, kvp.Key, FilterTextsByHeading(kvp.Value, headingContentMap)
            );
        }

        return assignmentSolutionMap;
    }

    private static List<string> ExtractAssignments(LLMRequestBuilder builder, string source)
    {
        // attach document to predefined prompt
        Prompt prompt = ExtractAssignmentsPrompt
                .UsingAttachment(new Attachment("Document", source));

        // ask the LLM to extract the assignments and extract the assignments from the output
        string output = builder.UsingPrompt(prompt)
            .Build()
            .Complete()
            // extract the unordered lists items
            .RawOutput;

        return ExtractUnorderedListItems(output);
    }

    private static List<string> SplitSource(string source)
    {
        // define HierarchichalTextSplitter inputs based on static values
        var splitIdentifiers = HierarchichalTextSplitter.MarkdownBasedHierarchy;
        var identifiersToAddToBeginnings = HierarchichalTextSplitter.MarkdownBasedAddToBeginnings;

        // splitting the text hierarchically until each part is below the maximum tokens
        return new HierarchichalTextSplitter(new OpenAITokenizer("gpt-3.5-turbo"), MaxInputTokens, splitIdentifiers, identifiersToAddToBeginnings)
            .Split(source);
    }

    private static string FindHeading(LLMRequestBuilder builder, string text)
    {
        // attach the text to the predefined prompt
        Prompt prompt = FindHeadingPrompt
               .UsingAttachment(new Attachment("Text", text, false));

        // Ask the LLM to find a heading for the given text
        return builder
           .UsingPrompt(prompt)
           .Build()
           .Complete()
           .RawOutput;
    }

    private static List<string> ExtractRelevantTextsByHeading(LLMRequestBuilder builder, string assignment, IEnumerable<string> headings)
    {
        // attach the assignment and the headings of the available texts onto the predefined prompt
        Prompt prompt = FindRelevantTextsPrompt
                .UsingAttachment(new Attachment("Assignment", assignment))
                .UsingAttachment(new Attachment("Available texts", String.Join("\n", headings)));

        // ask the LLM to find the necessary texts to complete an assignment
        string output = builder
            .UsingPrompt(prompt)
            .Build()
            .Complete()
            .RawOutput;

        return ExtractUnorderedListItems(output);
    }

    private static List<string> ExtractUnorderedListItems(string llmOutput)
    {
        // extract the unordered lists items
        return llmOutput.Split("\n")
            // filter by and remove identifier
            .Where(line => line.StartsWith("- "))
            .Select(line => line[2..])
            // remove empty lines
            .Where(line => line != "" && line != "\n")
            .ToList();
    }

    private static string AssignmentSolutionMapToString(Dictionary<string, string> assignmentSolutionMap)
    {
        var assignmentSolutions = assignmentSolutionMap.Select(
            assignmentKVP => "### " + assignmentKVP.Key + "\n\n" + assignmentKVP.Value
        );

        return String.Join("\n\n", assignmentSolutions);
    }

    private static string WriteAssignmentSolution(LLMRequestBuilder builder, string assignment, Dictionary<string, string> headingAndTextMap)
    {
        var prompt = WriteSolutionPrompt;
        foreach (var headingTextKvp in headingAndTextMap)
        {
            prompt.UsingAttachment(new Attachment(headingTextKvp.Key, headingTextKvp.Value, false));
        }

        // let the llm complete the assignment
        return builder
           .UsingPrompt(prompt)
           .Build()
           .Complete()
           .RawOutput;
    }

    private static Dictionary<string, string> FilterTextsByHeading(List<string> includedTextHeadings, Dictionary<string, string> headingContentMap)
    {
        var headingMap = new Dictionary<string, string>();

        foreach (var heading in includedTextHeadings)
        {
            headingMap[heading] = headingContentMap[heading];
        }

        return headingMap;
    }
}
