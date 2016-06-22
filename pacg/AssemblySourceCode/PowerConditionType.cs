using System;
using System.Runtime.InteropServices;

[Serializable, StructLayout(LayoutKind.Sequential)]
public struct PowerConditionType
{
    public bool Not;
    public PowerCondition Condition;
    public MetaBoolOperator Operator;
}

