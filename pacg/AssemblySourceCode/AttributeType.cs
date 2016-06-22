using System;

public enum AttributeType
{
    [StrRefAttr(8, "UI")]
    Charisma = 5,
    [StrRefAttr(4, "UI")]
    Constitution = 1,
    [StrRefAttr(5, "UI")]
    Dexterity = 2,
    [StrRefAttr(7, "UI")]
    Intelligence = 4,
    None = 6,
    [StrRefAttr(3, "UI")]
    Strength = 0,
    [StrRefAttr(6, "UI")]
    Wisdom = 3
}

