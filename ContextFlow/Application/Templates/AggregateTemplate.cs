﻿using ContextFlow.Application.Prompting;

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
            prompt.UsingAttachment(new Attachment(null, piece, false));
        }
        return prompt;
    }
}
