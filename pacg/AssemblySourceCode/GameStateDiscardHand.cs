using System;

public class GameStateDiscardHand : GameState
{
    public override void Enter()
    {
        base.Enter();
        if (Turn.Owner.Hand.Count > 0)
        {
            this.Refresh();
        }
        else
        {
            this.Proceed();
        }
    }

    protected override string GetHelpText()
    {
        int num = Turn.Character.GetNumberDiscardableCards() - Turn.Character.HandSize;
        if (num <= 0)
        {
            return StringTableManager.GetHelperText(0x45);
        }
        return string.Format(StringTableManager.GetHelperText(70), num);
    }

    public override bool IsActionAllowed(ActionType action, Card card)
    {
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (action == ActionType.Discard)
        {
            return true;
        }
        if (action == ActionType.Recharge)
        {
            return card.IsActionAllowed(action);
        }
        return base.IsActionAllowed(action, card);
    }

    public override bool IsState(GameStateType type) => 
        (base.IsState(type) || (type == GameStateType.Discard));

    public override void Proceed()
    {
        base.ProcessLayoutDecks();
        base.Message((string) null);
        Turn.State = GameStateType.ResetHand;
    }

    public override void Refresh()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Turn.Owner.IsOverHandSize())
            {
                window.ShowProceedButton(false);
                window.GlowLayoutDeck(ActionType.Discard, true);
            }
            else
            {
                window.ShowProceedButton(true);
                window.GlowLayoutDeck(ActionType.Discard, false);
            }
        }
        base.Message(this.GetHelpText());
    }

    public override GameStateType Type =>
        GameStateType.DiscardHand;
}

