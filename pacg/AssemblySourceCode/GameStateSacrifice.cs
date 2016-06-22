using System;

public class GameStateSacrifice : GameState
{
    protected TurnStateData data;
    private int numCardsAtStart;
    protected GuiWindowLocation window;

    public override void Cancel()
    {
        base.Message((string) null);
        base.HideTopCard(ActionType.Recharge, Turn.Character.Deck);
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        this.window = UI.Window as GuiWindowLocation;
        this.data = Turn.GetStateData();
        this.numCardsAtStart = 0;
        this.numCardsAtStart = this.GetNumMovedCards();
        if (Turn.Character.Deck.Count <= 0)
        {
            this.Proceed();
        }
        else
        {
            base.ShowTopCard(ActionType.Recharge, Turn.Character.Deck);
            if (Turn.Character.Deck.Count > 0)
            {
                Turn.Character.Deck[0].PlayedOwner = Turn.Character.ID;
            }
            this.Refresh();
        }
    }

    protected override string GetHelpText()
    {
        int num = this.data.NumCards - this.GetNumMovedCards();
        if (num == 1)
        {
            return string.Format(StringTableManager.GetHelperText(0x51), this.data.Actions[0], num);
        }
        return string.Format(StringTableManager.GetHelperText(0x52), this.data.Actions[0], num);
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
        return (num - this.numCardsAtStart);
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (this.data != null)
        {
            if ((this.window != null) && (this.GetNumMovedCards() >= this.data.NumCards))
            {
                return false;
            }
            if (Turn.Character.ID != this.data.Owner)
            {
                return false;
            }
            if (card.Deck != Turn.Character.Deck)
            {
                return false;
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

    public override void Proceed()
    {
        Turn.GotoStateDestination();
    }

    public override void Refresh()
    {
        if (this.window != null)
        {
            if ((this.GetNumMovedCards() >= this.data.NumCards) || (Turn.Character.Deck.Count <= 0))
            {
                base.Message((string) null);
                for (int i = 0; i < this.data.Actions.Length; i++)
                {
                    this.window.GlowLayoutDeck(this.data.Actions[i], false);
                }
                this.window.GlowLayoutDeck(ActionType.Recharge, false);
                this.Proceed();
            }
            else
            {
                base.Message(this.GetHelpText());
                this.window.ShowProceedButton(false);
                this.window.ShowCancelButton(true);
                for (int j = 0; j < this.data.Actions.Length; j++)
                {
                    this.window.GlowLayoutDeck(this.data.Actions[j], true);
                }
            }
        }
    }

    public override GameStateType Type =>
        GameStateType.Sacrifice;
}

