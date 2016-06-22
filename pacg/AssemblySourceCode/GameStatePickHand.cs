using System;

public class GameStatePickHand : GameStatePick
{
    private GuiWindowLocation window;

    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        base.data = Turn.GetStateData();
        this.window = UI.Window as GuiWindowLocation;
        if (base.data != null)
        {
            if (!base.data.Proceed)
            {
                if ((base.data.NumCards > 0) && ((base.GetNumCardsInDeck(base.GetDeck(base.data.Deck)) + base.GetNumCardsInDeck(Turn.Character.Hand)) >= base.data.NumCards))
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
                this.Refresh();
            }
        }
        else
        {
            this.Proceed();
        }
    }

    protected override string GetHelpText()
    {
        int num = base.data.MaxNumCards - base.GetNumMovedCards();
        if (base.data.Proceed)
        {
            num = Scenario.Current.Discard.Count - base.GetNumMovedCards();
        }
        if ((base.data.Actions.Length > 1) && base.data.Proceed)
        {
            return string.Format(StringTableManager.GetHelperText(0x49), base.data.Actions[0], base.data.Actions[1], num);
        }
        if (base.data.Proceed)
        {
            return string.Format(StringTableManager.GetHelperText(0x4b), base.data.Actions[0], num);
        }
        if (base.data.Actions.Length > 1)
        {
            return string.Format(StringTableManager.GetHelperText(0x4a), base.data.Actions[0], base.data.Actions[1], num);
        }
        return string.Format(StringTableManager.GetHelperText(0x4c), base.data.Actions[0], num);
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (base.data != null)
        {
            if ((this.window != null) && (this.window.GetNumCardsInLayout(action) >= base.data.MaxNumCards))
            {
                return false;
            }
            if (!base.data.Filter.Match(card))
            {
                return false;
            }
            if (Turn.Character.ID != base.data.Owner)
            {
                return false;
            }
            for (int i = 0; i < base.data.Actions.Length; i++)
            {
                if (base.data.Actions[i] == action)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Proceed()
    {
        if (base.data.Proceed)
        {
            Turn.BlackBoard.Set<int>("GameStatePickHand_NumMovedCards", base.GetNumMovedCards());
        }
        Turn.GotoStateDestination();
    }

    public override void Refresh()
    {
        if (base.data.Proceed && (this.window != null))
        {
            base.Message(this.GetHelpText());
            if (((base.GetNumMovedCards() >= base.data.MaxNumCards) && (base.data.MaxNumCards > 0)) || ((Turn.Character.Hand.Filter(base.data.Filter) <= 0) && (base.GetDeck(base.data.Deck).Filter(base.data.Filter) <= 0)))
            {
                this.window.ShowProceedButton(true);
                this.window.layoutBanish.Show(false);
            }
            else
            {
                this.window.ShowProceedButton(base.GetNumMovedCards() >= base.data.NumCards);
                this.window.layoutBanish.Show(true);
            }
        }
    }

    private void ShowSelectionPanel()
    {
        if (this.window != null)
        {
            this.window.layoutTray.Deck = base.GetDeck(base.data.Deck);
            this.window.layoutTray.TitleText = base.data.Deck.ToText();
            if (base.data.Filter == null)
            {
                base.data.Filter = CardFilter.Empty;
            }
            this.window.layoutTray.Pick = base.data.Filter;
            this.window.layoutTray.PickAmount = base.data.MaxNumCards;
            this.window.layoutTray.PickFromHand = true;
            this.window.layoutTray.Modal = false;
            this.window.layoutTray.Show(true);
            this.window.ShowCancelButton(false);
            this.window.ShowProceedButton(false);
        }
    }

    public override GameStateType Type =>
        GameStateType.PickHand;
}

