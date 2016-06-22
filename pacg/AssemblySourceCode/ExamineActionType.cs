using System;

public enum ExamineActionType
{
    [StrRefAttr(0x1b9, "UI")]
    Acquire = 2,
    [StrRefAttr(0x1bb, "UI")]
    Draw = 3,
    [StrRefAttr(350, "UI")]
    Encounter = 1,
    [StrRefAttr(0x1ba, "UI")]
    Evade = 4,
    None = 0,
    [StrRefAttr(0x10c, "UI")]
    Recharge = 5,
    [StrRefAttr(0x1da, "UI")]
    ToggleBottomToUnderTop = 6,
    [StrRefAttr(0x1db, "UI")]
    ToggleUnderTopToBottom = 7
}

