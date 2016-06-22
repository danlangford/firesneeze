using System;
using UnityEngine;

public class GuiLayoutReveal : GuiLayout
{
    [Tooltip("the layout manager in the scene which lays out the hand")]
    public GuiLayoutHand LayoutHand;
    private readonly float padx = 0.1f;

    public override ActionType GetActionType(Card card)
    {
        CardPower[] components = card.GetComponents<CardPower>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i].Action == ActionType.Display)
            {
                return ActionType.Display;
            }
        }
        return ActionType.Reveal;
    }

    public Vector3 GetCardPosition(Card card, int i, int count)
    {
        float num = this.GetCardWidth(card) + this.padx;
        Vector3 vector = base.transform.position - new Vector3(num * Geometry.GetMidPoint(count), 0f, 0f);
        return (vector + new Vector3(num * i, 0f, 0f));
    }

    private float GetCardWidth(Card card)
    {
        Vector3 localScale = card.transform.localScale;
        card.transform.localScale = this.Scale;
        float x = card.Size.x;
        card.transform.localScale = localScale;
        return x;
    }

    public override int IndexOf(int playedPower, Card playedPowerOwner)
    {
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if (((this.Deck[i].PlayedPower == playedPower) && (this.Deck[i].PlayedPowerOwner == playedPowerOwner)) && (this.Deck[i].Revealed || this.Deck[i].Displayed))
            {
                return i;
            }
        }
        return -1;
    }

    public override bool IsDeactivateOnDrag(Card card) => 
        ((Turn.State != GameStateType.Damage) && (Turn.State != GameStateType.Ambush));

    public override bool IsDeactivateOnDrop(Card card, GuiLayout layout)
    {
        if (((Turn.State != GameStateType.Damage) && (Turn.State != GameStateType.Ambush)) || ((layout == null) || (layout.GetActionType(card) != ActionType.Discard)))
        {
            if (layout != this)
            {
                return true;
            }
            for (int i = 0; i < card.PlayedPowers.Count; i++)
            {
                if (card.PlayedPowers[i].PlayedPowerOwner != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected override bool IsDeactivationNecessary(Card card, bool allCharacters)
    {
        if (card.Locked && card.Displayed)
        {
            return false;
        }
        return base.IsDeactivationNecessary(card, allCharacters);
    }

    public override bool OnGuiDrag(Card card) => 
        !card.Displayed;

    public override bool OnGuiDrop(Card card)
    {
        if (((this.LayoutHand == null) || !this.LayoutHand.IsDropPossible(card)) || (Turn.State == GameStateType.Share))
        {
            return false;
        }
        if (Turn.IsActionAllowed(ActionType.Reveal, card))
        {
            card.Revealed = true;
            Tutorial.Notify(TutorialEventType.CardRevealed);
        }
        else if (Turn.IsActionAllowed(ActionType.Display, card))
        {
            card.Displayed = true;
            Tutorial.Notify(TutorialEventType.CardDisplayed);
        }
        if (card.Deck != this.LayoutHand.Deck)
        {
            this.LayoutHand.Deck.Add(card);
        }
        this.LayoutHand.Refresh();
        if (card.Displayed)
        {
            Scenario.Current.OnCardPlayed(card);
            Turn.Character.OnCardPlayed(card);
            Location.Current.OnCardPlayed(card);
            Turn.Character.MarkCardType(card.Type, false);
        }
        return (card.Revealed || card.Displayed);
    }

    public override bool Validate(Deck deck)
    {
        bool flag = false;
        int number = Turn.Number;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Turn.Number = i;
            this.LayoutHand.Deck = Turn.Character.Hand;
            this.Deck = Turn.Character.Hand;
            flag |= base.Validate(Party.Characters[i].Hand);
            for (int j = 0; j < this.LayoutHand.Deck.Count; j++)
            {
                this.LayoutHand.Deck[j].Show(false);
            }
        }
        Turn.Number = number;
        this.LayoutHand.Deck = Turn.Character.Hand;
        this.Deck = Turn.Character.Hand;
        return (flag && this.AutoRefresh);
    }

    protected override bool Validate(Card card, ActionType action)
    {
        if (card.Displayed && card.Locked)
        {
            return false;
        }
        return base.Validate(card, action);
    }
}

