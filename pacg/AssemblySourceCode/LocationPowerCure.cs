using System;
using UnityEngine;

public class LocationPowerCure : LocationPower
{
    [Tooltip("dice used to compute the amount")]
    public DiceType[] Dice = new DiceType[1];
    [Tooltip("bonus added to the amount (total not per dice)")]
    public int DiceBonus;
    [Tooltip("Apothecary ends the turn setting the game to the reset state")]
    public bool EndsTurn = true;
    [Tooltip("Apothecary is the only cure I've seen shuffle cards from the top. When you cure, cure cards from this position. Currently not working for TopAndBottom or UnderTop")]
    public DeckPositionType HealPosition = DeckPositionType.Shuffle;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.MarkPowerActive(this, true);
        for (int i = 0; i < this.Dice.Length; i++)
        {
            Turn.Dice.Add(this.Dice[i]);
        }
        Turn.DiceBonus += this.DiceBonus;
        this.RefreshDicePanel();
        Turn.Checks = null;
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerCure_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerCure_Cure"));
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.State = GameStateType.Roll;
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.Character.Discard.Count <= 0)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return true;
    }

    private void LocationPowerCure_Cancel()
    {
        Turn.ReturnToReturnState();
        this.PowerEnd();
    }

    private void LocationPowerCure_Cure()
    {
        this.PowerEnd();
        Character character = Turn.Character;
        if (this.HealPosition == DeckPositionType.Shuffle)
        {
            character.Discard.Shuffle();
        }
        int num = Mathf.Clamp(Turn.DiceTotal, 0, character.Discard.Count);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Card[] cards = new Card[num];
            if (this.HealPosition == DeckPositionType.Bottom)
            {
                num = character.Discard.Count - num;
                int index = 0;
                for (int i = character.Discard.Count - 1; i >= num; i--)
                {
                    cards[index] = character.Discard[i];
                    index++;
                }
            }
            else
            {
                for (int j = num - 1; j >= 0; j--)
                {
                    cards[j] = character.Discard[j];
                }
            }
            window.Heal(character, cards, DeckPositionType.Shuffle);
        }
        float delay = 0.5f + (num * 0.2f);
        VisualEffect.Shuffle(delay, DeckType.Character);
        delay += 0.5f;
        LeanTween.delayedCall(delay, new Action(this.LocationPowerCure_Done));
    }

    private void LocationPowerCure_Done()
    {
        if (this.EndsTurn)
        {
            Turn.PopReturnState();
            Turn.State = GameStateType.Reset;
        }
        else
        {
            Turn.ReturnToReturnState();
        }
    }

    private void RefreshDicePanel()
    {
        Turn.DiceTarget = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Refresh();
        }
    }
}

