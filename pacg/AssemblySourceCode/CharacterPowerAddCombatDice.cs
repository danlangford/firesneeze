using System;
using UnityEngine;

public class CharacterPowerAddCombatDice : CharacterPower
{
    [Tooltip("is a card recharge/discard needed?")]
    public ActionType Action;
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;
    [Tooltip("short = help at this location; long = help at remote locations")]
    public DamageRangeType Range = DamageRangeType.Short;
    [Tooltip("additional traits to be added")]
    public TraitType[] Traits;

    public override void Activate()
    {
        Turn.MarkPowerActive(this, true);
        if (this.Action != ActionType.None)
        {
            base.PowerBegin();
            Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerAddCombatDice_Cancel"));
            Turn.SetStateData(new TurnStateData(this.Action, 1));
            Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerAddCombatDice_Finish"));
            Turn.State = GameStateType.Penalty;
        }
        else
        {
            this.AddDice();
        }
    }

    private void AddDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        Turn.AddTraits(this.Traits);
        Rules.ApplyCombatAdjustments();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }

    private void CharacterPowerAddCombatDice_Cancel()
    {
        Turn.State = GameStateType.Combat;
        this.PowerEnd();
    }

    private void CharacterPowerAddCombatDice_Finish()
    {
        this.AddDice();
        Turn.MarkPowerActive(this, true);
        Turn.State = GameStateType.Combat;
        this.PowerEnd();
    }

    private bool IsInCombat()
    {
        if (!Rules.IsCombatCheck())
        {
            return false;
        }
        if (((Turn.Card.Type != CardType.Monster) && (Turn.Card.Type != CardType.Villain)) && ((Turn.Card.Type != CardType.Henchman) || (Turn.Card.SubType == CardType.Barrier)))
        {
            return false;
        }
        return true;
    }

    public override bool IsShortcutAvailable() => 
        ((this.Action == ActionType.None) && (this.Range != DamageRangeType.Self));

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsRangeValid(base.Character, this.Range))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!this.IsInCombat())
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if ((this.Action != ActionType.None) && (Turn.Character.Hand.Count < 1))
        {
            return false;
        }
        return true;
    }
}

