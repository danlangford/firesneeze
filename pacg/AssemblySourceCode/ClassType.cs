using System;

public enum ClassType
{
    [StrRefAttr(14, "UI")]
    Barbarian = 1,
    [StrRefAttr(15, "UI")]
    Bard = 2,
    [StrRefAttr(0x10, "UI")]
    Cleric = 3,
    [StrRefAttr(0x11, "UI")]
    Druid = 4,
    [StrRefAttr(0x12, "UI")]
    Fighter = 5,
    [StrRefAttr(0x13, "UI")]
    Monk = 6,
    [StrRefAttr(0, "UI")]
    None = 0,
    [StrRefAttr(20, "UI")]
    Paladin = 7,
    [StrRefAttr(0x15, "UI")]
    Ranger = 8,
    [StrRefAttr(0x16, "UI")]
    Rogue = 9,
    [StrRefAttr(0x17, "UI")]
    Sorcerer = 10,
    [StrRefAttr(0x18, "UI")]
    Wizard = 11
}

