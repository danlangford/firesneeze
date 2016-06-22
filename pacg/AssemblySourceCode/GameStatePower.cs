using System;
using System.Runtime.CompilerServices;

public class GameStatePower : GameStatePenalty
{
    public override void Cancel()
    {
        Fulfilled = true;
        base.Cancel();
    }

    public override void Enter()
    {
        Fulfilled = false;
        base.Enter();
    }

    public override void Exit(GameStateType nextState)
    {
        Fulfilled = true;
        base.Exit(nextState);
    }

    public override void Refresh()
    {
        if (base.window != null)
        {
            if ((base.GetNumMovedCardsSinceStart() >= base.data.NumCards) || (Turn.Character.Hand.Count <= 0))
            {
                if ((base.GetNumMovedCardsSinceStart() < base.data.MaxNumCards) || (base.data.MaxNumCards < 0))
                {
                    base.Refresh();
                }
                else
                {
                    Fulfilled = true;
                    base.Message((string) null);
                    for (int i = 0; i < base.data.Actions.Length; i++)
                    {
                        base.window.GlowLayoutDeck(base.data.Actions[i], false);
                    }
                    this.Proceed();
                }
            }
            else
            {
                Fulfilled = false;
                base.Message(this.GetHelpText());
                for (int j = 0; j < base.data.Actions.Length; j++)
                {
                    base.window.GlowLayoutDeck(base.data.Actions[j], true);
                }
                base.window.ShowProceedButton(false);
                base.window.ShowCancelButton(true);
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
        GameStateType.Power;
}

