using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContextFlow.Application.TextUtil;

public class Prompt
{

    private string Action;

    private List<Attachement> Attachements;
    private Dictionary<string, string> Parameters;

    public bool ThrowExceptionOnUnfilled = true;

    public Prompt(string action)
    {
        Action = action;
    }

    public string ToPlainText()
    {
        return "";
    }

    public override string ToString()
    {
        return $"Prompt(Action=\"{Action}\", Attachements=\"{Attachements}\", Parameters=\"{Parameters}\")";
    }

    public void Validate()
    {
        
    }


}
