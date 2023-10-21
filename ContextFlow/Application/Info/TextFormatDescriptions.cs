using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public static class TextFormatDescriptions
{
    public static string plainText = "Plain text consisting of paragraphs.";
    public static string orderedList = "An ordered list: \n1. ...\n2. ...";
    public static string unorderedList = "An unordered list: \n- ...\n- ...";
    public static string headingSectionsOfUnorderedLists = "A section, each containing a heading (\"### Heading\") and an unordered List (\"- ...\")";
    public static string headingSectionsOfOrderedLists = "A section, each containing a heading (\"### Heading\") and an ordered List (\"1. ...\")";
}
