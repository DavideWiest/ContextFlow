namespace ContextFlow.Application.Prompting;

/// <summary>
/// An attachment of a Prompt with an optional name, a string-based content, and an option that determines if the string-representation is in-line.
/// If you want to access the attachments string representation, use ToPlainText().
/// Example attachment:
/// Name:
/// Content
/// Example inline attachment:
/// Name: Content
/// Example attachment without name:
/// Content
/// </summary>
public class Attachment
{
    /// <summary>
    /// The name of the attachment. This is optional but recommended. Choose a name that makes the context clear for the LLM.
    /// </summary>
    public string? Name { get; set; } = null;
    /// <summary>
    /// The content of the attachment. Must be a string.
    /// </summary>
    public string Content { get; set; }
    /// <summary>
    /// Option to determine if the string-representation is in a single line.
    /// </summary>
    public bool IsInline { get; set; } = false;

    public Attachment(string? name, string content, bool isInline)
    {
        Name = name;
        Content = content;
        IsInline = isInline;
    }

    public Attachment(string content, bool isInline)
    {
        Content = content;
        IsInline = isInline;
    }

    /// <summary>
    /// Returns the string representation of the Attachment
    /// </summary>
    /// <returns></returns>
    public string ToPlainText()
    {
        string namestr = GetNameString();
        string sep = !IsInline && namestr != null ? "\n" : "";
        return $"{namestr}{sep}{Content}";
    }

    private string GetNameString()
    {
        return Name != null && Name != string.Empty && Name != "" ? Name + ": " : string.Empty;
    }

    /// <summary>
    /// Returns a string that can be used to identify an Attachment. Don't confuse it with ToPlainText()
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Attachment(Name=\"{Name}\", Content=\"{Content}\", IsInline=\"{IsInline}\")";
    }

    /// <summary>
    /// Shallow copy using ICloneable
    /// </summary>
    /// <returns>The copied object. Use type-casting to convert it to a prompt</returns>
    public Attachment Clone()
    {
        return (Attachment)MemberwiseClone();
    }
}
