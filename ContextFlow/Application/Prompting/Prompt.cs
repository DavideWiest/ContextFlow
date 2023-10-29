namespace ContextFlow.Application.Prompting;

public class Prompt
{

    public string PromptAction { get; protected set; }

    public List<Attachment> Attachments { get; } = new();

    /// <summary>
    /// Creates a prompt without attachments. This has only the action-component set, which is valid if the action string has all the required context for the LLM.
    /// Most methods are in fluent-interface-style
    /// </summary>
    /// <param name="action"></param>
    public Prompt(string action)
    {
        PromptAction = action;
    }

    /// <summary>
    /// Adds many attachments
    /// </summary>
    /// <param name="attachments"></param>
    /// <returns>The modified prompt</returns>
    public Prompt UsingAttachments(IEnumerable<Attachment> attachments)
    {
        Attachments.AddRange(attachments);
        return this;
    }

    /// <summary>
    /// Adds one attachment
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns>The modified prompt</returns>
    public Prompt UsingAttachment(Attachment attachment)
    {
        Attachments.Add(attachment);
        return this;
    }

    /// <summary>
    /// Upserts an attachment: Will overwrite the first attachment with the same name as the inputted attachment, or insert it if there is no such match.
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
    public Prompt UpsertingAttachment(Attachment attachment)
    {
        var existingAttachment = Attachments.FirstOrDefault(a => a.Name == attachment.Name);

        if (existingAttachment != null)
        {
            // Update the existing attachment's content.
            existingAttachment.Content = attachment.Content;
        }
        else
        {
            // No existing attachment with the same name; add the new one.
            UsingAttachment(attachment);
        }

        return this;
    }

    /// <summary>
    /// Upserts an inline attachment with the name "Output format" which content is the given description
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public Prompt UsingOutputDescription(string description)
    {
        UpsertingAttachment(new Attachment("Output format", description, true));
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>string representation of the prompt</returns>
    public virtual string ToPlainText()
    {
        return PromptAction + (Attachments.Count > 0 ? "\n\n" : "") + string.Join("\n\n", Attachments.Select(a => a.ToPlainText()));
    }

    public override string ToString()
    {
        string attachmentstr = String.Join(", ", Attachments);
        return $"Prompt(PromptAction=\"{PromptAction}\", Attachments=[ {attachmentstr} ])";
    }

    /// <summary>
    /// Creates a shallow clone of this prompt
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Prompt Clone()
    {
        return (Prompt)MemberwiseClone();
    }
}
