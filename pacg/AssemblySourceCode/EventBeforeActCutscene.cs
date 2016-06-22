using System;
using UnityEngine;

public class EventBeforeActCutscene : Event
{
    [Tooltip("name of the cutscene file to display")]
    public string Cutscene;

    public override bool IsEventValid(Card card)
    {
        if (Game.GameMode == GameModeType.Quest)
        {
            return false;
        }
        if (Campaign.IsCardEncountered(card.ID))
        {
            return false;
        }
        if (!Cutscene.Exists(this.Cutscene))
        {
            return false;
        }
        return true;
    }

    public override void OnBeforeAct()
    {
        if (!this.IsEventValid(Turn.Card))
        {
            Event.Done();
        }
        else
        {
            Cutscene.Queue = this.Cutscene;
            Turn.State = GameStateType.HenchmanIntro;
        }
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

