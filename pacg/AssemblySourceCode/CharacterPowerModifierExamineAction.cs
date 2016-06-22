using System;
using UnityEngine;

public class CharacterPowerModifierExamineAction : BaseCharacterPowerMod
{
    [Tooltip("extra option for examine character powers")]
    public ExamineActionType ExamineAction;

    public override void Activate()
    {
        base.Activate();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Action = this.ExamineAction;
        }
    }

    public override bool IsValid() => 
        base.IsConditionValid(Turn.Card);
}

