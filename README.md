# ContextFlow

[![.NET](https://github.com/DavideWiest/ContextFlow/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/DavideWiest/ContextFlow/actions/workflows/dotnet-desktop.yml)

ContextFlow is a C# library that builds an abstraction above regular LLMs and enables more complex interaction with LLMs.

ContextFlow is infrastructure that 
1. decouples different modules from their abstractions to make code more flexible and 
2. provides easy ways to build complex prompt-pipelines through these abstractions

### Get Started
- [Download](https://www.nuget.org/packages/ContextFlow/) the package from nuget: `NuGet\Install-Package ContextFlow -Version 1.0.0`
- [Quickstart](https://github.com/DavideWiest/ContextFlow/wiki/Quickstart#walkthrough)
- [See demo](https://github.com/DavideWiest/ContextFlow/tree/master/Demo)

### Principles of ContextFlow
- Intuitive design
- Highly extensible
- Extensive Logging
- Strongly typed
- Preconfigured specific settings
- Asynchronous support

### Architectural principles
- Dependency injection whenever possible
- Project-wide fluent interface
- Abstract classes for replacable modules
- Private when possible, protected when useful, publicly assessible when necessary

# What is this good for?
LLMs are only as useful as how well they are used. 
As LLMs become more competent, the programmatic environment in which they operate must expand to fulfill different needs. This project is supposed to make that easier.

# How does ContextFlow organize interactions with LLMs?

### Core model
The topmost class is the `LLMRequest`, which is constructed with an  `LLMRequestBuilder`. All required data and configuration are dependency-injected into the request, which then handles the process of getting output and using defined extensions.

The required modules are
- `Prompt`: Consisting of an action, and a variable number of string-based attachments.
- `LLMConnection`: A connection to the LLM that returns the output and optionally additional data
- `RequestConfig`: Stores the extensions used and some behaviour.
- `LLMConfig`: Stores the LLM's settings (system message, frequency pentaly etc.) as well as constraints (maximum input tokens, maximum output tokens)

##### Async
Some modules have their async-counterparts: `LLMRequestAsync`, `RequestConfigAsync`, and `LLMConnectionAsync`

##### Custom exceptions
- `LLMConnectionException`: Something went wrong on the LLM-connection side
- `OutputOverflow`: The LLM's output was cut off because it reached the token limit
- `ÌnputOverflow`: The input limit was reached. (This gets thrown before the request happens)

### Extensions

##### Logger
The `CFLogger`-abstract class is a serilog-like interface. Conversely, the standard logger is a serilog-implementation. By default, it saves the messages into a file too.
##### Loader
Handles loading of the data. Must also implement a method that determines if a matching saved request exists. 
It's recommended to add an option to determine if only the prompt or also the llm-configuration should be considered when looking for a match.
##### Saver
Handles saving the data. 
##### FailStrategies
A `FailStrategy` handles a specific exception that occured, if it can. It gets the `LLMRequest` and optionally returns a `RequestResult`. 
Nesting FailStrategies that handle the same exception is possible but disrecommended.
### RequestResult
The returned object of both `LLMRequest` and `LLMRequestAsync` is a `RequestResult`. It contains the raw output, the reason why the LLM stopped its output, and an optional `RequestAdditionalData`-instance which got passed up from the `LLMConnection`
This result can be parsed (into a `ParsedRequestResult`).
##### Actions
Both `RequestResult` and it's parsed counterpart have an `Actions`/`AsyncActions`-property, that makes pipelining requests easier:

- `.Then(...) -> RequestResult`: Executes the next step in a pipeline of requests. It takes a function that builds the next `LLMRequest` from the current result, applies it to itself, and completes the request
- `.ThenConditional(...) -> RequestResult`: Returns itself it the condition does not match, or if it does, it applies a function like the one in `.Then(...)`.
- `.ThenBranching(...) -> IEnumerable<RequestResult>`: Like `.Then(...)` but the function builds a number of requests
- `.ThenBranchingConditional(...) -> (IEnumerable<RequestResult>, IEnumerable<RequestResult>`: Like `.ThenBranching(...)`, but separates the results that pass a condition from those that don't.

All synchronous actions that would experience a performance-gain from being asynchronous have that counterpart.
