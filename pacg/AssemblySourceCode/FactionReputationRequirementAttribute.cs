using System;
using System.Runtime.CompilerServices;

public class FactionReputationRequirementAttribute : StatRequirementAttribute
{
    public FactionReputationRequirementAttribute(string paramAxisName, string paramValueName, string paramFactionName) : base(paramAxisName, paramValueName, true)
    {
        this.ParamFactionName = paramFactionName;
    }

    public string ParamFactionName { get; private set; }
}

