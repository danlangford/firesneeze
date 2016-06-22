using System;

public class GameStateMove : GameState
{
    public override void Cancel()
    {
        Turn.GotoCancelDestination();
    }

    public override void Enter()
    {
        LocationPowerBaseMoveRestriction component = Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponent<LocationPowerBaseMoveRestriction>();
        if ((component != null) && component.IsValid())
        {
            Turn.EmptyLayoutDecks = false;
            component.Activate();
        }
        else if (base.IsCurrentState())
        {
            this.Proceed();
        }
    }

    public override void Proceed()
    {
        base.SaveRechargableCards();
        Turn.GotoStateDestination();
    }

    public override GameStateType Type =>
        GameStateType.Move;
}

