using System;
using UnityEngine;

public class LocationPowerDefeat : LocationPower
{
    [Tooltip("how much of the action needed in order to defeat the encountered card")]
    public int ActionAmount = 1;
    [Tooltip("action to be done in order to activate ability")]
    public ActionType ActionType = ActionType.Recharge;
    [Tooltip("the types of encountered this ability would work on")]
    public CardSelector EncounterSelector;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerEncounter_Cancel"));
        Turn.SetStateData(new TurnStateData(this.ActionType, this.ActionAmount));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerEncounter_Defeat"));
        Turn.State = GameStateType.Penalty;
    }

    public override bool IsValid()
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (this.EncounterSelector == null)
        {
            return false;
        }
        if (!this.EncounterSelector.Match(Turn.Card))
        {
            return false;
        }
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        return (Turn.State == GameStateType.Combat);
    }

    private void LocationPowerEncounter_Cancel()
    {
        this.PowerEnd();
        Turn.State = GameStateType.Combat;
    }

    private void LocationPowerEncounter_Defeat()
    {
        this.PowerEnd();
        Turn.LastCombatResult = CombatResultType.Win;
        Turn.State = GameStateType.Damage;
    }
}

