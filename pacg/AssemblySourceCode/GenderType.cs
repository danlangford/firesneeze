using System;

public enum GenderType
{
    [StrRefAttr(2, "UI")]
    Female = 2,
    [StrRefAttr(1, "UI")]
    Male = 1,
    [StrRefAttr(0, "UI")]
    None = 0
}

