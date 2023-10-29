namespace ContextFlow.Application.Prompting;

public class Prompt
{

    public string PromptAction { get; protected set; }

    public List<Attachment> Attachments { get; } = new();

    public Prompt(string action)
    {
        PromptAction = action;
    }
    public Prompt UsingAttachments(IEnumerable<Attachment> attachments)
    {
        Attachments.AddRange(attachments);
        return this;
    }

    public Prompt UsingAttachment(Attachment attachment)
    {
        Attachments.Add(attachment);
        return this;
    }

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

    public Prompt UsingOutputDescription(string description)
    {
        UpsertingAttachment(new Attachment("Output format", description, true));
        return this;
    }

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
    /// Shallow copy using memberwise-clone
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Prompt Clone()
    {
        return (Prompt)MemberwiseClone();
    }
}
