namespace ContextFlow.Domain;

/// <summary>
/// The configuration of the LLM. Be aware that not all settings are supported by all LLMs.
/// </summary>
public class LLMConfig
{
    public string ModelName;
    public string SystemMessage = "You are a helpful assistant.";
    public int MaxLength = 512;
    public int MaxTokensInput = 256;
    public double Temperature = 0.3;
    public double TopP = 1;
    public double FrequencyPentaly = 0;
    public double PresencePenalty = 0;

    public LLMConfig(string modelName, int maxTokensInput)
    {
        ModelName = modelName;
        MaxTokensInput = maxTokensInput;
    }

    public LLMConfig UsingSystemMessage(string message)
    {
        SystemMessage = message;
        return this;
    }

    public LLMConfig UsingTemperature(double temperature)
    {
        Temperature = temperature;
        return this;
    }

    public LLMConfig UsingMaxLength(int maxlength)
    {
        MaxLength = maxlength;
        return this;
    }

    public LLMConfig UsingTopP(double topp)
    {
        TopP = topp;
        return this;
    }

    public LLMConfig UsingFrequencyPenalty(double frequencypenalty)
    {
        FrequencyPentaly = frequencypenalty;
        return this;
    }

    public LLMConfig UsingPresencePenalty(double presencepenalty)
    {
        PresencePenalty = presencepenalty;
        return this;
    }

}
