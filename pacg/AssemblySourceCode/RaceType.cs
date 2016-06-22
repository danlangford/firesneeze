using System;

public enum RaceType
{
    [StrRefAttr(9, "UI")]
    Dwarf = 1,
    [StrRefAttr(10, "UI")]
    Elf = 2,
    [StrRefAttr(11, "UI")]
    Gnome = 3,
    [StrRefAttr(12, "UI")]
    Halfling = 4,
    [StrRefAttr(13, "UI")]
    Human = 5,
    [StrRefAttr(0, "UI")]
    None = 0
}

