using System;
using UnityEngine;

public class ScenarioPowerMove : ScenarioPower
{
    [Tooltip("message displayed when moving")]
    public StrRefType Message;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.Explore = false;
        Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "ScenarioMovePlayer_Cancel"));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "ScenarioMovePlayer_Done"));
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.messagePanel.Show(this.Message.ToString());
        }
        Turn.Target = Turn.Current;
        Location.Current.Move();
        if (this.Cancellable)
        {
            base.ShowCancelButton(true);
        }
    }

    public override bool IsValid()
    {
        if (!base.IsConditionValid(Turn.Card))
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (Turn.IsPowerActive(base.ID))
        {
            return false;
        }
        return this.IsLocationValid(Location.Current.ID);
    }

    private void ScenarioMovePlayer_Cancel()
    {
        this.PowerEnd();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowMap(false);
            window.messagePanel.Clear();
            window.ShowProceedEndTurnButton(true);
        }
    }

    private void ScenarioMovePlayer_Done()
    {
        this.PowerEnd();
        Turn.MarkPowerActive(this, true);
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.messagePanel.Clear();
            Turn.State = GameStateType.EndTurn;
        }
    }
}

