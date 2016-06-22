using System;

public enum CardType
{
    [StrRefAttr(0x19, "UI")]
    Ally = 1,
    [StrRefAttr(0x1a, "UI")]
    Armor = 2,
    [StrRefAttr(0x1b, "UI")]
    Barrier = 3,
    [StrRefAttr(0x1c, "UI")]
    Blessing = 4,
    [StrRefAttr(0x14c, "UI")]
    Character = 13,
    [StrRefAttr(0x1d, "UI")]
    Henchman = 5,
    [StrRefAttr(30, "UI")]
    Item = 6,
    [StrRefAttr(0x23, "UI")]
    Location = 11,
    [StrRefAttr(0x24, "UI")]
    Loot = 12,
    [StrRefAttr(0x1f, "UI")]
    Monster = 7,
    [StrRefAttr(0, "UI")]
    None = 0,
    [StrRefAttr(0x18e, "UI")]
    Scenario = 14,
    [StrRefAttr(0x20, "UI")]
    Spell = 8,
    [StrRefAttr(0x21, "UI")]
    Villain = 9,
    [StrRefAttr(0x22, "UI")]
    Weapon = 10
}

