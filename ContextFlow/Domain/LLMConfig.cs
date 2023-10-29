using ContextFlow.Application;

namespace ContextFlow.Domain;

/// <summary>
/// The configuration of the LLM. Be aware that not all settings are supported by all LLMs.
/// </summary>
public class LLMConfig
{
    public string ModelName { get; private set; }
    public string SystemMessage { get; private set; } = "You are a helpful assistant.";
    public int MaxTotalTokens { get; private set; } = 1024;
    public int MaxInputTokens { get; private set; } = 512;
    public double Temperature { get; private set; } = 0.3;
    public double TopP { get; private set; } = 1;
    public double FrequencyPenalty { get; private set; } = 0;
    public double PresencePenalty { get; private set; } = 0;
    public int NumOutputs { get; private set; } = 1;

    public LLMConfig(string modelName, int maxTotalTokens = 1024, int maxInputTokens = 512)
    {
        ModelName = modelName;
        MaxTotalTokens = maxTotalTokens;
        MaxInputTokens = maxInputTokens;
    }

    public LLMConfig UsingSystemMessage(string message)
    {
        SystemMessage = message;
        return this;
    }

    public LLMConfig UsingTemperature(double temperature)
    {
        if (temperature <= 0)
            throw new InvalidDataException($"Temperature must be a positive double [inputted temperature={temperature}]");

        Temperature = temperature;
        return this;
    }

    public LLMConfig UsingMaxTotalTokens(int maxTotalTokens)
    {
        if (maxTotalTokens <= 0)
            throw new InvalidDataException($"Maximum total tokens must be a positive integer [inputted maxTotalTokens={maxTotalTokens}]");
        if (maxTotalTokens <= MaxInputTokens)
            throw new InvalidDataException($"Maximum total tokens must be greater than maximum input tokens [MaxInputTokens={MaxInputTokens}, inputted maxTotalTokens={maxTotalTokens}]");


        MaxTotalTokens = maxTotalTokens;
        return this;
    }

    public LLMConfig UsingMaxInputTokens(int maxInputTokens)
    {
        if (maxInputTokens <= 0)
            throw new InvalidDataException($"Maximum input tokens must be a positive integer [inputted maxInputTokens={maxInputTokens}]");
        if (MaxTotalTokens <= maxInputTokens)
            throw new InvalidDataException($"Maximum total tokens must be greater than maximum input tokens [MaxTotalTokens={MaxTotalTokens}, inputted maxInputTokens={maxInputTokens}]");

        MaxInputTokens = maxInputTokens;
        return this;
    }

    public LLMConfig UsingTopP(double topp)
    {
        if (topp <= 0)
            throw new InvalidDataException($"Top p must be a positive double [inputted topp={topp}]");

        TopP = topp;
        return this;
    }

    public LLMConfig UsingFrequencyPenalty(double frequencypenalty)
    {
        if (frequencypenalty <= 0)
            throw new InvalidDataException($"Frequency penalty must be a positive double [inputted frequencypenalty={frequencypenalty}]");

        FrequencyPenalty = frequencypenalty;
        return this;
    }

    public LLMConfig UsingPresencePenalty(double presencepenalty)
    {
        if (presencepenalty <= 0)
            throw new InvalidDataException($"Presence penalty must be a positive double [inputted presencepenalty={presencepenalty}]");

        PresencePenalty = presencepenalty;
        return this;
    }
    public LLMConfig UsingNumOutputs(int numOutputs)
    {
        if (numOutputs <= 0)
            throw new InvalidDataException($"Number of outputs must be a positive integer [inputted numOutputs={numOutputs}]");

        NumOutputs = numOutputs;
        return this;
    }

    public override string ToString()
    {
        return $"LLMConfig(ModelName=\"{ModelName}\", SystemMessage=\"{SystemMessage}\", MaxTotalTokens={MaxTotalTokens}, MaxInputTokens={MaxInputTokens}, Temperature={Temperature}, TopP={TopP}, FrequencyPenalty={FrequencyPenalty}, PresencePenalty={PresencePenalty}, NumOutputs={NumOutputs})";
    }
}
