using System;
using UnityEngine;

public class LocationCloseActionCheck : LocationPower
{
    [Tooltip("all the checks that the character can do")]
    public SkillCheckValueType[] Checks;
    [Tooltip("source deck")]
    public DeckType From = DeckType.Discard;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("selects the cards that can be picked")]
    public CardSelector Selector;
    [Tooltip("if true, the destination deck will shuffle after the move finishes")]
    public bool Shuffle = true;
    [Tooltip("destination deck")]
    public DeckType To = DeckType.Character;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedButton(false);
            if (Turn.Dice.Count == 0)
            {
                this.SetupDicePanel(window.dicePanel);
                window.dicePanel.Refresh();
            }
        }
        Turn.SetStateData(new TurnStateData(this.Message));
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationCloseActionCheck_Roll"));
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
        if (this.Selector == null)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        Deck deck = Turn.Character.GetDeck(this.From);
        if (deck == null)
        {
            return false;
        }
        if (this.Selector.Filter(deck) <= 0)
        {
            return false;
        }
        return true;
    }

    private void LocationCloseActionCheck_Finish()
    {
        this.PowerEnd();
        Deck deck = Turn.Character.GetDeck(this.From);
        if (this.Shuffle)
        {
            VisualEffect.Shuffle(this.From);
            deck.Shuffle();
        }
        Turn.State = GameStateType.Done;
    }

    private void LocationCloseActionCheck_Roll()
    {
        if (Turn.IsResolveSuccess())
        {
            Turn.SetStateData(new TurnStateData(this.From.ToActionType(), this.Selector.ToFilter(), this.To.ToActionType(), 1));
            Turn.PushStateDestination(new TurnStateCallback(this, "LocationCloseActionCheck_Finish"));
            Turn.State = GameStateType.Pick;
        }
        else
        {
            this.PowerEnd();
            Turn.State = GameStateType.Done;
        }
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        if (dicePanel != null)
        {
            SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(this.Checks);
            dicePanel.SetCheck(Location.Current.Card, this.Checks, bestSkillCheck.skill);
            Turn.DiceTarget = bestSkillCheck.Rank;
            dicePanel.Refresh();
        }
    }
}

