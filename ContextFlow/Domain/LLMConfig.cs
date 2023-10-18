namespace ContextFlow.Domain;

/// <summary>
/// The configuration of the LLM. Be aware that not all settings are supported by all LLMs.
/// </summary>
public class LLMConfig
{
    public string ModelName;
    public string SystemMessage = "You are a helpful assistant.";
    public double Temperature = 0.3;
    public double MaxLength = 256;
    public double TopP = 1;
    public double FrequencyPentaly = 0;
    public double PresencePenalty = 0;

    public LLMConfig(string modelName)
    {
        ModelName = modelName;
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

    public LLMConfig UsingMaxLength(double maxlength)
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
