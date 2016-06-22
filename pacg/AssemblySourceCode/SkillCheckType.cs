using System;

public enum SkillCheckType
{
    [StrRefAttr(0x26, "UI")]
    Acrobatics = 2,
    [StrRefAttr(0x25, "UI")]
    Arcane = 1,
    [StrRefAttr(0x27, "UI")]
    Charisma = 3,
    [StrRefAttr(40, "UI")]
    Combat = 4,
    [StrRefAttr(0x29, "UI")]
    Constitution = 5,
    [StrRefAttr(0x2a, "UI")]
    Craft = 6,
    [StrRefAttr(0x2b, "UI")]
    Dexterity = 7,
    [StrRefAttr(0x2c, "UI")]
    Diplomacy = 8,
    [StrRefAttr(0x2d, "UI")]
    Disable = 9,
    [StrRefAttr(0x2e, "UI")]
    Divine = 10,
    [StrRefAttr(0x2f, "UI")]
    Fortitude = 11,
    [StrRefAttr(0x30, "UI")]
    Intelligence = 12,
    [StrRefAttr(0x31, "UI")]
    Knowledge = 13,
    [StrRefAttr(0x33, "UI")]
    Melee = 15,
    [StrRefAttr(0, "UI")]
    None = 0,
    [StrRefAttr(50, "UI")]
    Perception = 14,
    [StrRefAttr(0x34, "UI")]
    Ranged = 0x10,
    [StrRefAttr(0x36, "UI")]
    Stealth = 0x12,
    [StrRefAttr(0x37, "UI")]
    Strength = 0x13,
    [StrRefAttr(0x35, "UI")]
    Survival = 0x11,
    [StrRefAttr(0x38, "UI")]
    Wisdom = 20
}

