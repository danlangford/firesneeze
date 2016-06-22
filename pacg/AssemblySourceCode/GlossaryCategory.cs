using System;

public enum GlossaryCategory
{
    AttemptingACheck = 5,
    [StrRefAttr(0x114, "UI")]
    CoreConcepts = 0,
    [StrRefAttr(0x51, "rules")]
    DoNotForget = 4,
    [StrRefAttr(0x115, "UI")]
    Phases = 1,
    [StrRefAttr(0x116, "UI")]
    Terms = 2,
    [StrRefAttr(0x49, "rules")]
    Tips = 3
}

