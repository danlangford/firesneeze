using System;

public class GameStatePick : GameState
{
    protected TurnStateData data;

    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        this.data = Turn.GetStateData();
        if (this.data != null)
        {
            this.Refresh();
            if (!this.data.Proceed)
            {
                if (this.GetNumCardsInDeck(this.GetDeck(this.data.Deck)) >= this.data.NumCards)
                {
                    this.ShowSelectionPanel();
                }
                else
                {
                    this.Proceed();
                }
            }
            else
            {
                this.ShowSelectionPanel();
            }
        }
        else
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            base.Message((string) null);
            window.Pause(false);
            window.layoutTray.Show(false);
            window.ShowCancelButton(false);
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
        if (action == ActionType.Top)
        {
            return Turn.Character.Recharge;
        }
        if (action == ActionType.Bury)
        {
            return Turn.Character.Bury;
        }
        if (action == ActionType.Damage)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                return window.layoutDiscard.Deck;
            }
        }
        return null;
    }

    protected override string GetHelpText()
    {
        if (!string.IsNullOrEmpty(this.data.Message))
        {
            return this.data.Message;
        }
        int num = this.data.MaxNumCards - this.GetNumMovedCards();
        string str = (this.data.MaxNumCards < 0) ? string.Empty : num.ToString();
        bool flag = ((num > 1) || (this.data.MaxNumCards < 0)) || (this.data.MaxNumCards > 1);
        if (this.data.Proceed && (this.data.MaxNumCards < 0))
        {
            return string.Format(StringTableManager.GetHelperText(0x4d), this.data.Deck.ToText());
        }
        if ((this.data.Proceed && (this.data.NumCards != this.data.MaxNumCards)) && !flag)
        {
            return string.Format(StringTableManager.GetHelperText(0x4e), str, this.data.Deck.ToText());
        }
        return string.Format(StringTableManager.GetHelperText(0x4f), str, this.data.Deck.ToText());
    }

    protected int GetNumCardsInDeck(Deck deck)
    {
        int num = 0;
        if (deck != null)
        {
            num = deck.Filter(this.data.Filter);
        }
        return num;
    }

    protected int GetNumMovedCards()
    {
        int num = 0;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            for (int i = 0; i < this.data.Actions.Length; i++)
            {
                num += window.GetNumCardsInLayout(this.data.Actions[i]);
            }
        }
        return num;
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (this.data != null)
        {
            if ((card.Deck.Layout != null) && (card.Deck.Layout.CardAction != this.data.Deck))
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < this.data.Actions.Length; i++)
            {
                if (this.data.Actions[i] == action)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    public override void Refresh()
    {
        base.Message(this.GetHelpText());
        if (this.data.Proceed)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                if (((this.GetNumMovedCards() >= this.data.MaxNumCards) && (this.data.MaxNumCards > 0)) || (this.GetDeck(this.data.Deck).Filter(this.data.Filter) <= 0))
                {
                    window.ShowProceedButton(true);
                    window.layoutBanish.Show(false);
                }
                else
                {
                    window.ShowProceedButton(this.GetNumMovedCards() >= this.data.NumCards);
                    window.layoutBanish.Show(true);
                }
            }
        }
    }

    private void ShowSelectionPanel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Pause(!this.data.Proceed);
            window.layoutTray.Deck = this.GetDeck(this.data.Deck);
            window.layoutTray.Layout = window.GetLayoutDeck(this.data.Deck.ToDeckType());
            window.layoutTray.TitleText = this.data.Deck.ToText();
            if (this.data.Filter == null)
            {
                this.data.Filter = CardFilter.Empty;
            }
            window.layoutTray.Pick = this.data.Filter;
            window.layoutTray.PickAmount = (this.data.MaxNumCards >= 0) ? this.data.MaxNumCards : 0x7fffffff;
            window.layoutTray.Modal = !this.data.Proceed;
            window.layoutTray.Show(true);
            window.ShowCancelButton(false);
            window.ShowProceedButton(false);
        }
    }

    public override GameStateType Type =>
        GameStateType.Pick;
}

