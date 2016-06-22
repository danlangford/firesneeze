using System;
using System.Runtime.CompilerServices;

public class ObjectReputationRequirementAttribute : StatRequirementAttribute
{
    public ObjectReputationRequirementAttribute(string paramAxisName, string paramValueName, string paramObjectName) : base(paramAxisName, paramValueName, true)
    {
        this.ParamObjectName = paramObjectName;
    }

    public string ParamObjectName { get; private set; }
}

