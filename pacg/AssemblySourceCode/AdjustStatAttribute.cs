using System;
using System.Runtime.CompilerServices;

public class AdjustStatAttribute : Attribute
{
    public AdjustStatAttribute(string paramTypeName, string paramValueName)
    {
        this.ParamTypeName = paramTypeName;
        this.ParamValueName = paramValueName;
        this.ParamLabelName = string.Empty;
    }

    public AdjustStatAttribute(string paramTypeName, string paramValueName, string paramLabelName)
    {
        this.ParamTypeName = paramTypeName;
        this.ParamValueName = paramValueName;
        this.ParamLabelName = paramLabelName;
    }

    public string ParamLabelName { get; private set; }

    public string ParamTypeName { get; private set; }

    public string ParamValueName { get; private set; }
}

