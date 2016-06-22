using System;

public class BlockReturnFromLocation : Block
{
    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutSummoner.Clear();
        }
        string iD = Turn.BlackBoard.Get<string>("LastLocation");
        Turn.Owner.Location = iD;
        Location.Load(iD);
        if (window != null)
        {
            window.shadePanel.Hide();
        }
        Turn.Current = Turn.InitialCharacter;
        Turn.Number = Turn.InitialCharacter;
        Turn.PushStateDestination(Turn.PopReturnState());
        Turn.State = GameStateType.Recharge;
    }
}

