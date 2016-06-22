using System;

public class IteratorAid : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Turn.Iterators.Remove(TurnStateIteratorType.Aid);
        Turn.SwitchType = SwitchType.None;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedAidButton(false);
        }
    }

    public override void Invoke()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowProceedAidButton(true);
        }
    }

    protected override bool IsRefreshAllowed() => 
        false;

    public override bool Next() => 
        base.NextCharacterInParty();

    public override void Start()
    {
        base.Start();
        Turn.SwitchType = SwitchType.AidAll;
        if (this.Next())
        {
            Game.UI.SwitchPanel.Show(true);
        }
        else
        {
            this.End();
        }
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Aid;
}

