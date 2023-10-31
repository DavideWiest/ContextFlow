namespace ContextFlow.Domain;

/// <summary>
/// 
/// </summary>
public enum FinishReason
{
    /// <summary>
    /// The token-limit prevented the LLM from generating more output
    /// </summary>
    Overflow,
    /// <summary>
    /// The LLM stopped its response itself. The usual/desired case.
    /// </summary>
    Stop,
    /// <summary>
    /// Unknown or other reason why the LLM stopped. 
    /// Triggers no events internally, but does not indicate that the LLM output length was correct either.
    /// </summary>
    Unknown
}
