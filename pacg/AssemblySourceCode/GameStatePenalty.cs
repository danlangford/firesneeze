using System;
using System.Runtime.CompilerServices;

public class GameStatePenalty : GameState
{
    protected TurnStateData data;
    private readonly ActionType[] PossibleActions = new ActionType[] { ActionType.Banish, ActionType.Bury, ActionType.Discard, ActionType.Recharge };
    protected GuiWindowLocation window;

    public override void Cancel()
    {
        base.HideTopCard(ActionType.Recharge, Turn.Character.Deck);
        this.window.layoutHand.Refresh();
        Turn.GotoCancelDestination();
        if (!base.IsCurrentState())
        {
            Fulfilled = true;
        }
    }

    public override void Enter()
    {
        Fulfilled = false;
        this.window = UI.Window as GuiWindowLocation;
        this.data = Turn.GetStateData();
        if ((this.data != null) && (this.window != null))
        {
            this.LockPreExisting(true);
            this.ShowTopCard();
            this.window.layoutHand.Refresh();
            this.window.dicePanel.Refresh();
            this.window.ShowCancelButton(false);
            Turn.CheckBoard.Set<int>("PenaltyStartHand", Turn.Character.GetNumberDiscardableCards());
            int num = Turn.Character.Hand.Filter(this.data.Filter);
            if ((this.data.MaxNumCards > num) && !Rules.IsCloseInProgress())
            {
                this.data.MaxNumCards = num;
            }
            if (((this.data.NumCards > 0) || (this.data.MaxNumCards != 0)) && (Turn.Character.GetNumberDiscardableCards() > 0))
            {
                Turn.CheckBoard.Set<int>("PenaltyStart", this.GetNumMovedCards());
                this.Refresh();
            }
            else
            {
                this.Proceed();
            }
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        Fulfilled = true;
        this.LockPreExisting(false);
        base.Message((string) null);
        this.window = UI.Window as GuiWindowLocation;
        if (this.window != null)
        {
            this.window.ShowCancelButton(false);
            this.window.layoutHand.Refresh();
        }
    }

    protected override string GetHelpText()
    {
        if (!string.IsNullOrEmpty(this.data.Message))
        {
            return this.data.Message;
        }
        int numberDiscardableCards = this.data.MaxNumCards - this.GetNumMovedCardsSinceStart();
        if ((this.data.MaxNumCards < 0) || (this.data.MaxNumCards == 0x7fffffff))
        {
            numberDiscardableCards = Turn.Character.GetNumberDiscardableCards();
        }
        if (this.data.Proceed && (this.data.Actions.Length > 1))
        {
            return string.Format(StringTableManager.GetHelperText(0x49), this.data.Actions[0], this.data.Actions[1], numberDiscardableCards);
        }
        if (this.data.Proceed)
        {
            return string.Format(StringTableManager.GetHelperText(0x4b), this.data.Actions[0], numberDiscardableCards);
        }
        if (this.data.Actions.Length > 1)
        {
            return string.Format(StringTableManager.GetHelperText(0x4a), this.data.Actions[0], this.data.Actions[1], numberDiscardableCards);
        }
        return string.Format(StringTableManager.GetHelperText(0x4c), this.data.Actions[0], numberDiscardableCards);
    }

    protected int GetNumMovedCards()
    {
        int num = 0;
        if (this.window != null)
        {
            for (int i = 0; i < this.data.Actions.Length; i++)
            {
                num += this.window.GetNumCardsInLayout(this.data.Actions[i]);
            }
        }
        return num;
    }

    protected int GetNumMovedCardsSinceStart() => 
        (this.GetNumMovedCards() - Turn.CheckBoard.Get<int>("PenaltyStart"));

    private Card GetPenaltyCard()
    {
        if (this.window != null)
        {
            for (int i = 0; i < this.data.Actions.Length; i++)
            {
                GuiLayout layoutDeck = this.window.GetLayoutDeck(this.data.Actions[i]);
                if (layoutDeck != null)
                {
                    if (this.data.Actions[i] == ActionType.Reveal)
                    {
                        for (int k = 0; k < layoutDeck.Deck.Count; k++)
                        {
                            if (layoutDeck.Deck[k].Revealed)
                            {
                                return layoutDeck.Deck[k];
                            }
                        }
                    }
                    for (int j = 0; j < layoutDeck.Deck.Count; j++)
                    {
                        if (!layoutDeck.Deck[j].Locked)
                        {
                            return layoutDeck.Deck[j];
                        }
                    }
                }
            }
        }
        return null;
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (this.data != null)
        {
            if ((Turn.CheckBoard.Get<int>("PenaltyStartHand") - Turn.Character.GetNumberDiscardableCards()) >= this.data.MaxNumCards)
            {
                return false;
            }
            if (Fulfilled)
            {
                return false;
            }
            if (!this.data.Filter.Match(card))
            {
                return false;
            }
            if (Turn.Character.ID != this.data.Owner)
            {
                return false;
            }
            if (card.Displayed)
            {
                return false;
            }
            if (card.Deck != Turn.Character.Hand)
            {
                bool flag = false;
                if (card.Deck == Turn.Character.Deck)
                {
                    for (int j = 0; j < this.data.Actions.Length; j++)
                    {
                        if (this.data.Actions[j] == ActionType.FromTheTop)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            for (int i = 0; i < this.data.Actions.Length; i++)
            {
                if (this.data.Actions[i] == action)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override bool IsProceedAllowed() => 
        (this.data.Owner == Turn.Character.ID);

    private void LockPreExisting(bool lockCard)
    {
        if (lockCard)
        {
            Turn.Character.LockInDisplayed(lockCard);
        }
        for (int i = 0; i < this.PossibleActions.Length; i++)
        {
            Deck deck = this.window.GetLayoutDeck(this.PossibleActions[i]).Deck;
            if (deck != null)
            {
                for (int j = 0; j < deck.Count; j++)
                {
                    deck[j].Locked = lockCard;
                }
            }
        }
    }

    public override void Proceed()
    {
        Card penaltyCard = this.GetPenaltyCard();
        if (penaltyCard != null)
        {
            Turn.CheckBoard.Set<string>("PenaltyGuid", penaltyCard.GUID.ToString());
        }
        if ((penaltyCard != null) && (Turn.CheckBoard.Get<int>("PenaltyStartHand") == Turn.Character.GetNumberDiscardableCards()))
        {
            Turn.CheckBoard.Set<bool>("PenaltyFromDeck", true);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                penaltyCard.Show(CardSideType.Front);
                switch (penaltyCard.Layout.CardAction)
                {
                    case ActionType.Discard:
                        window.Discard(penaltyCard);
                        break;

                    case ActionType.Bury:
                        window.Bury(penaltyCard);
                        break;
                }
            }
        }
        if (Turn.Evade)
        {
            Turn.State = GameStateType.Post;
        }
        else
        {
            Turn.GotoStateDestination();
        }
    }

    public override void Refresh()
    {
        if (this.window != null)
        {
            if (((this.GetNumMovedCardsSinceStart() >= this.data.MaxNumCards) && (this.data.MaxNumCards > 0)) || (Turn.Character.GetNumberDiscardableCards() <= 0))
            {
                Fulfilled = true;
                base.Message((string) null);
                this.window.ShowProceedButton(true);
            }
            else
            {
                Fulfilled = false;
                base.Message(this.GetHelpText());
                this.window.ShowProceedButton(this.data.Proceed);
            }
        }
    }

    private void ShowTopCard()
    {
        for (int i = 0; i < this.data.Actions.Length; i++)
        {
            if (this.data.Actions[i] == ActionType.FromTheTop)
            {
                base.ShowTopCard(ActionType.Recharge, Turn.Character.Deck);
                break;
            }
        }
    }

    public static bool Fulfilled
    {
        [CompilerGenerated]
        get => 
            <Fulfilled>k__BackingField;
        [CompilerGenerated]
        private set
        {
            <Fulfilled>k__BackingField = value;
        }
    }

    public override GameStateType Type =>
        GameStateType.Penalty;
}

