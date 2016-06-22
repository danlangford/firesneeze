using System;
using UnityEngine;

public class ScenarioPowerPenaltyMove : ScenarioPower
{
    [Tooltip("location ID to move to after penalty")]
    public string Destination;
    [Tooltip("number of penalty cards")]
    public int PenaltyAmount = 2;
    [Tooltip("what type of penalty is this? discard, etc.")]
    public ActionType PenaltyType = ActionType.Discard;

    public override void Activate()
    {
        base.PowerBegin();
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "ScenarioPowerPenaltyMove_Cancel"));
        Turn.SetStateData(new TurnStateData(this.PenaltyType, this.PenaltyAmount));
        Turn.PushStateDestination(new TurnStateCallback(TurnStateCallbackType.Scenario, "ScenarioPowerPenaltyMove_Move"));
        Turn.State = GameStateType.Penalty;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(true);
            window.dicePanel.Clear();
        }
    }

    public override bool IsValid()
    {
        if (Turn.Owner.Hand.Count < this.PenaltyAmount)
        {
            return false;
        }
        if (!Rules.IsTurnOwner())
        {
            return false;
        }
        if (!this.IsLocationValid(Location.Current.ID))
        {
            return false;
        }
        return Rules.IsEvadePossible(Turn.Card);
    }

    private void ScenarioPowerPenaltyMove_Cancel()
    {
        this.PowerEnd();
        Turn.ReturnToReturnState();
    }

    private void ScenarioPowerPenaltyMove_Move()
    {
        this.PowerEnd();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ProcessRechargableCards();
            window.ProcessLayoutDecks();
            window.dicePanel.Clear();
            window.messagePanel.Clear();
            window.layoutLocation.Show(false);
        }
        Turn.Card.Show(false);
        Turn.Evade = true;
        Turn.Explore = false;
        Turn.Character.Location = this.Destination;
        if (Turn.Number == Turn.InitialCharacter)
        {
            Turn.End = true;
        }
        Turn.State = GameStateType.Damage;
    }

    public override PowerType Type =>
        PowerType.Evade;
}

