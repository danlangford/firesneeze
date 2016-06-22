using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class StatRequirementAttribute : Attribute
{
    public StatRequirementAttribute(string paramTypeName, string paramValueName, bool isPersonalityRep = false)
    {
        this.ParamTypeName = paramTypeName;
        this.ParamValueName = paramValueName;
        this.IsPersonalityReputation = isPersonalityRep;
    }

    public bool IsPersonalityReputation { get; private set; }

    public string ParamTypeName { get; private set; }

    public string ParamValueName { get; private set; }
}

