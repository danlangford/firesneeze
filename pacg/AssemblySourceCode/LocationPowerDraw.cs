using System;

public class LocationPowerDraw : LocationPower
{
    public override void Activate()
    {
        base.PowerBegin(0.5f);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Draw(Turn.Character.Deck.Draw());
            window.ShowCancelButton(false);
            Turn.PopCancelDestination();
            Turn.Character.LockInDisplayed(true);
            if (Turn.State == GameStateType.ConfirmProceed)
            {
                Turn.Proceed();
            }
        }
        string name = "CharPlayedCard_" + Turn.Character.ID;
        if (Turn.BlackBoard.Get<string>(name) != null)
        {
            Turn.BlackBoard.Set<string>(name, null);
        }
    }

    public override bool IsValid()
    {
        if (Turn.State == GameStateType.EndTurn)
        {
            return false;
        }
        if (Turn.Character.Deck.Count == 0)
        {
            return false;
        }
        string name = "CharPlayedCard_" + Turn.Character.ID;
        return (Turn.BlackBoard.Get<string>(name) != null);
    }
}

