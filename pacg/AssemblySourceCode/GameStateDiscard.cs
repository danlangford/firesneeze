using System;

public class GameStateDiscard : GameState
{
    public override void Cancel()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.CancelAllPowers(false, true);
        }
        if (Turn.PeekCancelDestination() != null)
        {
            Turn.BlackBoard.Set<int>("CancelDiscardDestination", 0);
            Turn.Discard = false;
            Turn.End = false;
            base.Message((string) null);
            Turn.DestructiveActionsCount = 0;
            Turn.GotoCancelDestination();
        }
    }

    private GameStateType CancelDiscardDestination()
    {
        GameStateType type = Turn.BlackBoard.Get<int>("CancelDiscardDestination");
        if (type != GameStateType.None)
        {
            return type;
        }
        return GameStateType.Finish;
    }

    public override void Enter()
    {
        base.Enter();
        if (Rules.IsCancelOutOfDiscardPossible())
        {
            Turn.PushCancelDestination(this.CancelDiscardDestination());
        }
        Turn.Phase = TurnPhaseType.Discard;
        if (Turn.Character.Hand.Count > 0)
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
        int num = Turn.Character.Hand.Count - Turn.Character.HandSize;
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
        return ((action == ActionType.Discard) || ((action == ActionType.Recharge) && card.IsActionAllowed(action)));
    }

    public override void Proceed()
    {
        base.ProcessLayoutDecks();
        base.Message((string) null);
        Turn.State = GameStateType.Reset;
    }

    public override void Refresh()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            if (Rules.IsCancelOutOfDiscardPossible())
            {
                window.ShowCancelButton(true);
            }
            if (!Turn.Character.IsOverHandSize())
            {
                window.ShowProceedEndButton(true);
                window.GlowLayoutDeck(ActionType.Discard, false);
            }
            else
            {
                window.ShowProceedEndButton(false);
                window.GlowLayoutDeck(ActionType.Discard, true);
            }
            base.Message(this.GetHelpText());
        }
    }

    public override GameStateType Type =>
        GameStateType.Discard;
}

