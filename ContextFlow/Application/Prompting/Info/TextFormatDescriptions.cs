using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.Prompting.Info;

public static class TextFormatDescriptions
{
    public static readonly string plainText = "Plain text consisting of paragraphs.";
    public static readonly string orderedList = "An ordered list: \n1. ...\n2. ...";
    public static readonly string unorderedList = "An unordered list: \n- ...\n- ...";
    public static readonly string headingSectionsOfUnorderedLists = "A section, each containing a heading (\"### Heading\") and an unordered List (\"- ...\")";
    public static readonly string headingSectionsOfOrderedLists = "A section, each containing a heading (\"### Heading\") and an ordered List (\"1. ...\")";
}
