using System;
using UnityEngine;

public class CardPowerReplenishBlessing : CardPower
{
    [Tooltip("banish when finished with this power")]
    public bool BanishAfterUse = true;
    [Tooltip("the deck to select cards from")]
    public ActionType From;
    [Tooltip("string reference for helper text")]
    public StrRefType Message;
    [Tooltip("the selector of cards to pick")]
    public CardSelector Selector;
    [Tooltip("the deck to send cards to")]
    public ActionType To;

    public override void Activate(Card card)
    {
        if (this.IsPowerActivationAllowed(card))
        {
            this.ReplenishBlessings_Pick();
        }
    }

    public override void Deactivate(Card card)
    {
        if (this.IsPowerDeactivationAllowed(card))
        {
            card.ReturnCard(card);
            Turn.PopStateDestination();
            Turn.GotoCancelDestination();
        }
    }

    protected Deck GetDeck(ActionType action)
    {
        if (action == ActionType.Discard)
        {
            return Turn.Character.Discard;
        }
        if (action == ActionType.Recharge)
        {
            return Turn.Character.Recharge;
        }
        if (action == ActionType.Bury)
        {
            return Turn.Character.Bury;
        }
        return null;
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
        if (Scenario.Current.Discard.Count <= 0)
        {
            return false;
        }
        if ((this.Selector.Filter(this.GetDeck(this.From)) <= 0) && (this.Selector.Filter(Turn.Character.Hand) <= 0))
        {
            return false;
        }
        if ((Turn.State != GameStateType.Finish) && (Turn.State != GameStateType.Setup))
        {
            return false;
        }
        return true;
    }

    private void ReplenishBlessings_Cancel()
    {
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void ReplenishBlessings_Finish()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            Scenario.Current.Discard.Shuffle();
            int num = Turn.BlackBoard.Get<int>("GameStatePickHand_NumMovedCards");
            if (num > 0)
            {
                int num2 = Mathf.Clamp(num, 1, Scenario.Current.Discard.Count);
                for (int i = 0; i < num2; i++)
                {
                    Scenario.Current.Blessings.Add(Scenario.Current.Discard[UnityEngine.Random.Range(0, Scenario.Current.Discard.Count)]);
                    window.blessingsPanel.Increment(Scenario.Current.Blessings.Count);
                }
            }
            window.blessingsPanel.Shuffle();
            window.ShowCancelButton(false);
            Campaign.Box.Add(base.Card, false);
            Turn.EmptyLayoutDecks = true;
            Turn.ReturnToReturnState();
        }
    }

    private void ReplenishBlessings_Pick()
    {
        TurnStateData data = new TurnStateData(this.From, this.Selector.ToFilter(), this.To, 0, Scenario.Current.Discard.Count) {
            Message = StringTableManager.Get(this.Message)
        };
        Turn.EmptyLayoutDecks = false;
        Turn.PushReturnState(Turn.State);
        Turn.PushCancelDestination(new TurnStateCallback(base.Card, "ReplenishBlessings_Cancel"));
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "ReplenishBlessings_Finish"));
        Turn.State = GameStateType.PickHand;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutBanish.Show(true);
            window.Refresh();
        }
    }
}

