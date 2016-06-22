using System;
using UnityEngine;

public class CharacterPowerSneak : CharacterPower
{
    [Tooltip("dice added to the check")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the check (total not per dice)")]
    public int DiceBonus;

    public override void Activate()
    {
        base.PowerBegin();
        if (this.Cancellable)
        {
            Turn.EmptyLayoutDecks = false;
        }
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerSneakAttack_Cancel"));
        Turn.SetStateData(new TurnStateData(ActionType.Discard, ActionType.Recharge, 1));
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerSneakAttack_Finish"));
        Turn.State = GameStateType.Power;
    }

    private void AddDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
    }

    private void CharacterPowerSneakAttack_Cancel()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.State = GameStateType.Combat;
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    private void CharacterPowerSneakAttack_Finish()
    {
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.AddDice();
            Turn.DiceBonus += this.DiceBonus;
            string guid = Turn.CheckBoard.Get<string>("PenaltyGuid");
            if (guid != null)
            {
                bool flag = this.SetPowerInfo(guid);
                if (Turn.CheckBoard.Get<bool>("SneakDiscard") && flag)
                {
                    this.AddDice();
                }
            }
            window.dicePanel.Refresh();
        }
        Turn.State = GameStateType.Combat;
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }

    public override void Deactivate()
    {
        base.Deactivate();
        this.RemoveDice();
        Turn.DiceBonus -= this.DiceBonus;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            GuiLayoutStack layoutDiscard;
            if (Turn.CheckBoard.Get<bool>("SneakDiscard"))
            {
                this.RemoveDice();
                layoutDiscard = window.layoutDiscard;
            }
            else
            {
                layoutDiscard = window.layoutRecharge;
            }
            int index = layoutDiscard.IndexOf(base.Character.Powers.IndexOf(this), null);
            if (index >= 0)
            {
                Card card = layoutDiscard.Deck[index];
                if (Turn.Character == base.Character)
                {
                    window.layoutHand.OnGuiDrop(card);
                }
                else
                {
                    base.Character.Hand.Add(card);
                }
            }
            window.dicePanel.Refresh();
        }
    }

    protected override bool IsPowerValid()
    {
        if (!Rules.IsCombatCheck())
        {
            return false;
        }
        if (!Rules.IsDiceRollPossible())
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Location.CountCharactersAtLocation(Turn.Character.Location) > 1)
        {
            return false;
        }
        return base.IsPowerValid();
    }

    public override bool IsValid()
    {
        if (Turn.Character.Hand.Count < 1)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return this.IsPowerValid();
    }

    private void RemoveDice()
    {
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Remove(this.Dice[i]);
        }
    }

    private bool SetPowerInfo(string Guid)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (this.SetPowerInfo(window.GetLayoutDeck(ActionType.Discard), Guid))
            {
                Turn.CheckBoard.Set<bool>("SneakDiscard", true);
                return true;
            }
            if (this.SetPowerInfo(window.GetLayoutDeck(ActionType.Recharge), Guid))
            {
                Turn.CheckBoard.Set<bool>("SneakDiscard", false);
                return true;
            }
        }
        return false;
    }

    private bool SetPowerInfo(GuiLayout layout, string Guid)
    {
        for (int i = 0; i < layout.Deck.Count; i++)
        {
            if (layout.Deck[i].GUID.ToString().Equals(Guid))
            {
                layout.Deck[i].SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
                return true;
            }
        }
        return false;
    }
}

