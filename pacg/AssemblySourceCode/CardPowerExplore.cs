using System;
using UnityEngine;

public class CardPowerExplore : CardPower
{
    [Tooltip("if the explored card matches the selector, this is the disposition ")]
    public DispositionType Restriction;
    [Tooltip("if the explore card matches this selector, the restriction happens")]
    public CardSelector RestrictionSelector;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            if (Rules.IsExplorePromptNecessary())
            {
                GuiWindowLocation window = UI.Window as GuiWindowLocation;
                if (window != null)
                {
                    Turn.EmptyLayoutDecks = false;
                    Turn.PushReturnState();
                    window.Popup.Clear();
                    window.Popup.SetDeckPosition(base.Action);
                    window.Popup.Add(StringTableManager.GetUIText(0x107), new TurnStateCallback(card, "CancelActivation"));
                    window.Popup.Add(StringTableManager.GetUIText(0x1b0), new TurnStateCallback(card, "ConfirmActivation"));
                    Turn.State = GameStateType.Popup;
                }
            }
            else
            {
                this.ActivateExplorePower();
            }
            base.Activate(card);
        }
    }

    private void ActivateExplorePower()
    {
        Turn.Character.MarkCardType(base.Card.Type, true);
        Turn.Explore = true;
        if (this.Restriction != DispositionType.None)
        {
            EffectExploreRestriction e = new EffectExploreRestriction(base.Card.ID, 1, this.Restriction, this.RestrictionSelector.ToFilter());
            Turn.Character.ApplyEffect(e);
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowExploreButton(true);
        }
    }

    private void CancelActivation()
    {
        Turn.EmptyLayoutDecks = false;
        base.Card.ReturnCard(base.Card);
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
        UI.Window.Refresh();
    }

    private void ConfirmActivation()
    {
        Turn.EmptyLayoutDecks = true;
        Turn.ReturnToReturnState();
        this.ActivateExplorePower();
        UI.Window.Refresh();
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            Turn.Character.MarkCardType(card.Type, false);
            if (this.Restriction != DispositionType.None)
            {
                Effect e = Turn.Character.GetEffect(EffectType.ExploreRestriction);
                if (e != null)
                {
                    Turn.Character.RemoveEffect(e);
                }
            }
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.ShowExploreButton(false);
            }
            if (Turn.State == GameStateType.Popup)
            {
                Turn.ReturnToReturnState();
            }
            else
            {
                Turn.Explore = false;
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
        if (Location.Current.Deck.Count <= 0)
        {
            return false;
        }
        bool flag = false;
        for (int i = 0; i < Location.Current.Deck.Count; i++)
        {
            if ((Location.Current.Deck[i].Blocker != BlockerType.Movement) || (i >= Turn.CountExplores))
            {
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            return false;
        }
        return (Turn.State == GameStateType.Finish);
    }
}

