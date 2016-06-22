using System;
using UnityEngine;

public class EventEncounteredCutscene : Event
{
    [Tooltip("names of the cutscenes to show in locations (blank means any location)")]
    public CutsceneValueType[] Cutscenes;
    [Tooltip("specifies the card which triggers this event")]
    public CardSelector Selector;

    private string GetCutsceneForLocation(string id)
    {
        for (int i = 0; i < this.Cutscenes.Length; i++)
        {
            if (string.IsNullOrEmpty(this.Cutscenes[i].Location) || (this.Cutscenes[i].Location == id))
            {
                return this.Cutscenes[i].Cutscene;
            }
        }
        return null;
    }

    private bool IsEncounteredBefore(Card card)
    {
        string name = "_Enc_" + card.ID + "_at_" + Location.Current.ID;
        bool flag = Scenario.Current.BlackBoard.Get<bool>(name);
        Scenario.Current.BlackBoard.Set<bool>(name, true);
        return flag;
    }

    public override bool IsEventValid(Card card)
    {
        if (Game.GameMode == GameModeType.Quest)
        {
            return false;
        }
        if (!this.Selector.Match(card))
        {
            return false;
        }
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        if (!this.IsEncounteredBefore(card))
        {
            return false;
        }
        string cutsceneForLocation = this.GetCutsceneForLocation(Location.Current.ID);
        if (cutsceneForLocation == null)
        {
            return false;
        }
        if (!Cutscene.Exists(cutsceneForLocation))
        {
            return false;
        }
        return true;
    }

    public override void OnCardEncountered(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            Cutscene.Queue = this.GetCutsceneForLocation(Location.Current.ID);
            Turn.State = GameStateType.HenchmanIntro;
            Event.DonePost(GameStateType.HenchmanIntro);
        }
    }

    public override bool Stateless =>
        !this.IsEventValid(Turn.Card);

    public override EventType Type =>
        EventType.OnCardEncountered;
}

