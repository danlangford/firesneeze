using System;

public enum ProficencyType
{
    [StrRefAttr(0x91, "UI")]
    HeavyArmor = 2,
    [StrRefAttr(0xa1, "UI")]
    LightArmor = 1,
    [StrRefAttr(0, "UI")]
    None = 0,
    Weapons = 3
}

