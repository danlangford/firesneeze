using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPowerExchange : CharacterPower
{
    [Tooltip("help text displayed for this power")]
    public StrRefType Message;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.EmptyLayoutDecks = false;
        Turn.SetStateData(this.CreateDiscardFilter());
        Turn.PushReturnState();
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerExchange_Pick"));
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerExchange_Cancel"));
        Turn.State = GameStateType.Power;
    }

    private void CharacterPowerExchange_Cancel()
    {
        this.PowerEnd();
        this.GlowHandLayout(false);
        Card cardPlayed = base.GetCardPlayed(ActionType.Discard);
        if (cardPlayed != null)
        {
            cardPlayed.RemovePowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
            cardPlayed.ReturnCard(cardPlayed);
            UI.Window.Refresh();
        }
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void CharacterPowerExchange_Finish()
    {
        if (Turn.Character.Hand.Count > 0)
        {
            Turn.Character.Hand[0].Clear();
        }
        Turn.MarkPowerActive(this, true);
        Turn.ReturnToReturnState();
        this.GlowHandLayout(false);
        this.PowerEnd();
    }

    private void CharacterPowerExchange_Pick()
    {
        Card cardPlayed = base.GetCardPlayed(ActionType.Discard);
        if (cardPlayed != null)
        {
            cardPlayed.SetPowerInfo(base.Character.Powers.IndexOf(this), base.Character.ID);
        }
        CardFilter filter = new CardFilter {
            CardTypes = new CardType[] { this.GetDiscardCardType() }
        };
        TurnStateData data = new TurnStateData(ActionType.Discard, filter, ActionType.Draw, 1) {
            Message = this.Message.ToString()
        };
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(this, "CharacterPowerExchange_Finish"));
        Turn.PushCancelDestination(new TurnStateCallback(this, "CharacterPowerExchange_Cancel"));
        Turn.EmptyLayoutDecks = true;
        Turn.State = GameStateType.Pick;
        this.GlowHandLayout(true);
    }

    private int CountMatchingTypes(Deck deck1, Deck deck2)
    {
        int num = 0;
        for (int i = 0; i < deck1.Count; i++)
        {
            for (int j = 0; j < deck2.Count; j++)
            {
                if ((deck2[j].Type == deck1[i].Type) && (deck2[j].ID != deck1[i].ID))
                {
                    num++;
                }
            }
        }
        return num;
    }

    private TurnStateData CreateDiscardFilter()
    {
        TurnStateData data = new TurnStateData(ActionType.Discard, 1) {
            Message = this.Message.ToString(),
            Filter = new CardFilter()
        };
        List<CardType> list = new List<CardType>(Constants.NUM_CARD_TYPES);
        for (int i = 0; i < Turn.Character.Discard.Count; i++)
        {
            CardType item = Turn.Character.Discard[i].Type;
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
        data.Filter.CardTypes = list.ToArray();
        return data;
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    private CardType GetDiscardCardType()
    {
        CardType none = CardType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if ((window != null) && (window.layoutDiscard.Deck.Count > 0))
        {
            none = window.layoutDiscard.Deck[0].Type;
        }
        return none;
    }

    private void GlowHandLayout(bool isGlowing)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutHand.Glow(isGlowing);
        }
    }

    public override bool IsValid()
    {
        if (this.CountMatchingTypes(Turn.Character.Discard, Turn.Character.Hand) <= 0)
        {
            return false;
        }
        if (Turn.Character.GetNumberDiscardableCards() <= 0)
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        return base.IsConditionValid(Turn.Card);
    }
}

