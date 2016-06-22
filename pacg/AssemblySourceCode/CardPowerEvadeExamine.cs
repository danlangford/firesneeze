using System;
using UnityEngine;

public class CardPowerEvadeExamine : CardPower
{
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("true means that cards can be dragged from/to the bottom of the deck")]
    public bool ModifyBottom;
    [Tooltip("true means that cards can be dragged from/to the top of the deck")]
    public bool ModifyTop;
    [Tooltip("number of cards to examine")]
    public int Number = 1;
    [Tooltip("examine from the top or bottom of the deck")]
    public DeckPositionType Position = DeckPositionType.Top;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            Turn.Card.Disposition = DispositionType.None;
            Turn.Disposed = true;
            Turn.Evade = true;
            base.ShowDice(false);
            Turn.Character.MarkCardType(card.Type, true);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                Turn.Validate = true;
                window.Validate();
            }
            Turn.ClearCheckData();
            this.ShowExamineTray(true);
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Card.Disposition = DispositionType.None;
            base.ShowDice(true);
            Turn.Disposed = false;
            Turn.Evade = false;
            Turn.Character.MarkCardType(card.Type, false);
            this.ShowExamineTray(false);
            if (Turn.State == GameStateType.Horde)
            {
                base.ShowEncounterButton(true);
            }
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!base.IsAidPossible(card))
        {
            return false;
        }
        if (Turn.Character.IsCardTypeMarked(card.Type))
        {
            return false;
        }
        if ((card.Type == CardType.Spell) && Rules.IsImmune(Turn.Card, card))
        {
            return false;
        }
        return Rules.IsEvadePossible(Turn.Card);
    }

    public override bool IsPowerDeactivationAllowed(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return base.IsPowerDeactivationAllowed(card);
    }

    private void ShowExamineTray(bool isVisible)
    {
        if (Rules.IsCardSummons(Turn.Card))
        {
            Turn.Disposed = false;
            Turn.Card.Disposition = DispositionType.Banish;
            base.ShowProceedButton(true);
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutExamine.Mode = ExamineModeType.Reveal;
                window.layoutExamine.Source = DeckType.Location;
                window.layoutExamine.Number = Mathf.Min(this.Number, this.Deck.Count);
                window.layoutExamine.RevealPosition = this.Position;
                window.layoutExamine.ModifyTop = this.ModifyTop;
                window.layoutExamine.ModifyBottom = this.ModifyBottom;
                window.layoutExamine.Finish = true;
                Turn.EmptyLayoutDecks = false;
                Turn.PushReturnState();
                Turn.SetStateData(new TurnStateData(this.Message));
                Turn.State = GameStateType.Examine;
                window.layoutLocation.Show(!isVisible);
            }
        }
    }

    private Deck Deck =>
        Location.Current.Deck;

    public override PowerType Type =>
        PowerType.Evade;
}

