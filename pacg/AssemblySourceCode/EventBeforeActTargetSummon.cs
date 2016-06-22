using System;
using UnityEngine;

public class EventBeforeActTargetSummon : Event
{
    [Tooltip("text displayed in the target window")]
    public StrRefType Message;
    [Tooltip("the ID of the monster to summon; use $Turn.Card to repeat the current card")]
    public string Monster;
    [Tooltip("used to summon a horde type summon at the target player's location or all open locations, or everywhere?\t\t\t\t\t")]
    public LocationType SummonRange;
    [Tooltip("determines who can be targeted to get the summon")]
    public TargetType TargetRange = TargetType.AllAtLocation;

    public override void OnBeforeAct()
    {
        Turn.SummonsType = SummonsType.Target;
        Turn.SummonsMonster = SummonsSelector.GetSummonsMonster(this.Monster);
        Turn.SummonsLocation = this.SummonRange;
        this.SummonMonster_Start();
    }

    private void SummonMonster_Proceed()
    {
        Turn.EmptyLayoutDecks = true;
        if (Turn.Target != Turn.Number)
        {
            Turn.SwitchCharacter(Turn.Target);
            Turn.Current = Turn.Number;
        }
        Turn.State = GameStateType.Horde;
        Event.Done();
    }

    private void SummonMonster_Start()
    {
        if (Rules.IsTargetRequired(this.TargetRange))
        {
            this.SummonMonster_Target();
        }
        else
        {
            Turn.Target = Turn.Current;
            this.SummonMonster_Proceed();
        }
    }

    private void SummonMonster_Target()
    {
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "SummonMonster_Proceed"));
        Turn.TargetType = this.TargetRange;
        GameStateTarget.DisplayText = this.Message.ToString();
        Turn.State = GameStateType.Target;
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(false);
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

