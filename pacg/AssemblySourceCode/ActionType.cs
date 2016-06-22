using System;

public enum ActionType
{
    [StrRefAttr(0x10f, "UI")]
    Banish = 6,
    [StrRefAttr(270, "UI")]
    Bury = 5,
    Damage = 8,
    [StrRefAttr(0x10d, "UI")]
    Discard = 4,
    [StrRefAttr(0x10a, "UI")]
    Display = 1,
    [StrRefAttr(0x1bb, "UI")]
    Draw = 13,
    FromTheTop = 12,
    [StrRefAttr(0x110, "UI")]
    Give = 7,
    [StrRefAttr(0, "UI")]
    None = 0,
    [StrRefAttr(0x10c, "UI")]
    Recharge = 3,
    [StrRefAttr(0x10b, "UI")]
    Reveal = 2,
    Share = 9,
    [StrRefAttr(0x1ba, "UI")]
    Shuffle = 11,
    [StrRefAttr(0x1bd, "UI")]
    Top = 10
}

