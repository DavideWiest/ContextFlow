using ContextFlow.Application.Prompting;
using ContextFlow.Application.Templates.Util;
using ContextFlow.Domain;

namespace ContextFlow.Application.Templates;

public class AggregateTemplate : CFTemplate
{
    public override string Action { get; } = "Aggregate these texts into a single one of the same format.";

    private IEnumerable<string> TextPieces;

    public AggregateTemplate(IEnumerable<string> textPieces)
    {
        TextPieces = textPieces;
    }

    protected override Prompt ConfigurePrompt(Prompt prompt)
    {
        foreach (var piece in TextPieces)
        {
            prompt.UsingAttachment(null, piece);
        }
        return prompt;
    }
}
