using System;
using UnityEngine;

public class BlockTargetEncounter : BlockEncounterCard
{
    [Tooltip("should just be the current location since having people at other locations might be problematic")]
    public TargetType Range;
    [Tooltip("text the target panel should display")]
    public StrRefType TargetText;

    private void BlockTargetEncounter_Finish()
    {
        Turn.Current = Turn.Target;
        Turn.Number = Turn.Current;
        base.Invoke();
    }

    public override void Invoke()
    {
        if (base.IsValid())
        {
            if (Rules.IsTargetRequired(this.Range))
            {
                Turn.TargetType = this.Range;
                if (base.Card != null)
                {
                    Turn.PushStateDestination(new TurnStateCallback(base.Card.ID, "BlockTargetEncounter_Finish"));
                }
                GameStateTarget.DisplayText = this.TargetText.ToString();
                Turn.State = GameStateType.Target;
            }
            else
            {
                Turn.Target = Turn.Current;
                this.BlockTargetEncounter_Finish();
            }
        }
    }
}

