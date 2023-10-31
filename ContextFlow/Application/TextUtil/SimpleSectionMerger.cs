namespace ContextFlow.Application.TextUtil;

/// <summary>
/// Merger that can be used in an overflowstrategy, which merges first-level sections together, using the given identifiers (the substring, e.g. a heading)
/// If a section occurs multiple times in a single sting, they will be merged together as usual.
/// The key of the sectionIdentifiersWithJoinStrings-attribute is the string which connects the sections
/// 
/// example usage:
/// var merger = new SimpleSectionMerger(new() { { "A: ", "\n" }, { "B: ", " - " } }}
/// string output = merger.Merge(new() { "A: apple apple B: banana banana", "B: pineapple A: strawberry B: avocado" })
/// "A: apple apple\nstrayberry B: banana banana - pineapple - avocado"
/// </summary>
public class SimpleSectionMerger : TextMerger
{
    private readonly Dictionary<string, string> SectionIdentifiersWithJoinStrings;
    private readonly string StartIdentifier = "$§-=?&";
    private readonly string EndIdentifier = "&%=!-";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sectionIdentifiersWithJoinStrings">A dictionary that contains the identifiers of a section, with the appropriate join-string with which the different parts of a section are merged</param>
    public SimpleSectionMerger(Dictionary<string, string> sectionIdentifiersWithJoinStrings)
    {
        SectionIdentifiersWithJoinStrings = sectionIdentifiersWithJoinStrings;
    }

    public override string Merge(IEnumerable<string> inputs)
    {
        Dictionary<string, List<string>> groupedSections = new();


        foreach (var input in inputs)
        {
            string taggedInput = TagIdentifiers(input);

            // Find all existing sections using their identifiers
            foreach (var section in taggedInput.Split(StartIdentifier))
            {
                groupedSections = UpdateGroupedSections(groupedSections, section);
            }
        }

        // Merge sections inside groupedSections together
        var mergedSections = groupedSections.Select(pair =>
        {
            var key = pair.Key;
            var sectionContents = string.Join(SectionIdentifiersWithJoinStrings[key], pair.Value);
            return $"{key}{sectionContents}";
        });

        // Convert the resulting sections into a single string
        return string.Join(" ", mergedSections);
    }

    private Dictionary<string, List<string>> UpdateGroupedSections(Dictionary<string, List<string>> groupedSections, string section)
    {
        if (SectionIdentifiersWithJoinStrings.Keys.Any(section.StartsWith))
        {
            if (!groupedSections.ContainsKey(section.Split(EndIdentifier)[0]))
                groupedSections[section.Split(EndIdentifier)[0]] = new List<string>();

            groupedSections[section.Split(EndIdentifier)[0]].Add(section.Split(EndIdentifier)[1].Trim());
        }

        return groupedSections;
    }

    private string TagIdentifiers(string input)
    {
        foreach (var identifier in SectionIdentifiersWithJoinStrings.Keys)
        {
            input = input.Replace(identifier, StartIdentifier + identifier + EndIdentifier);
        }
        return input;
    }
}
