using System;
using UnityEngine;

public class EventBeforeActRollPenalty : Event
{
    [Tooltip("number of penalty cards")]
    public int Amount = 1;
    [Tooltip("type of penalty cards; none means any")]
    public CardType CardType;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("type of penalty to be performed by the player")]
    public ActionType Penalty = ActionType.Discard;

    private void ClearDicePanel()
    {
        Turn.ClearCheckData();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Clear();
        }
    }

    private void EventEncounteredRollPenalty_Discard()
    {
        Turn.SetStateData(new TurnStateData(ActionType.Discard, this.Amount));
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredRollPenalty_Done"));
        Turn.State = GameStateType.Penalty;
    }

    private void EventEncounteredRollPenalty_Done()
    {
        Turn.State = GameStateType.Combat;
        Event.Done();
    }

    private void EventEncounteredRollPenalty_Resolve()
    {
        if (!Turn.IsResolveSuccess())
        {
            this.ClearDicePanel();
            Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredRollPenalty_Discard"));
            Turn.State = GameStateType.Recharge;
        }
        else
        {
            this.ClearDicePanel();
            Turn.PushStateDestination(GameStateType.Combat);
            Turn.State = GameStateType.Recharge;
            Event.Done();
        }
    }

    public override void OnBeforeAct()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.SetupDicePanel(window.dicePanel);
        }
        Turn.EmptyLayoutDecks = false;
        Turn.PushStateDestination(new TurnStateCallback(base.Card, "EventEncounteredRollPenalty_Resolve"));
        Turn.State = GameStateType.Roll;
    }

    private void SetupDicePanel(GuiPanelDice dicePanel)
    {
        SkillCheckValueType bestSkillCheck = Turn.Character.GetBestSkillCheck(this.Checks);
        dicePanel.SetCheck(Turn.Card, this.Checks, bestSkillCheck.skill);
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

