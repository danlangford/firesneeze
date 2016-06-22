using System;
using UnityEngine;

public class EventPostHordeDefeatAll : Event
{
    [Tooltip("if true immediately dispose of this card post horde if it's undefeated else wait after you act to set disposition")]
    public bool Immediate = true;

    public override bool IsEventImplemented(EventType type) => 
        ((!this.Immediate && (type == EventType.OnPostAct)) || ((type == EventType.OnCardEncountered) || base.IsEventImplemented(type)));

    public override bool IsEventValid(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            return false;
        }
        return true;
    }

    public override void OnCardEncountered(Card card)
    {
        Turn.NumCombatUndefeats = 0;
        Turn.NumCombatEvades = 0;
        base.OnCardEncountered(card);
    }

    public override void OnPostAct()
    {
        if (!this.Immediate)
        {
            this.SetDisposition(base.Card);
        }
        base.OnPostAct();
    }

    public override void OnPostHorde(Card card)
    {
        if (!this.IsEventValid(card))
        {
            Event.Done();
        }
        else
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutLocation.Show(true);
                window.layoutLocation.Refresh();
            }
            if (this.Immediate)
            {
                this.SetDisposition(card);
                Turn.PushStateDestination(GameStateType.Dispose);
                Turn.State = GameStateType.Recharge;
            }
            else
            {
                Turn.State = GameStateType.Ambush;
            }
            Event.Done();
        }
    }

    private void SetDisposition(Card card)
    {
        if ((Turn.NumCombatUndefeats + Turn.NumCombatEvades) > 0)
        {
            card.Disposition = DispositionType.Shuffle;
            Turn.LastCombatResult = CombatResultType.Lose;
        }
        else
        {
            card.Disposition = DispositionType.Banish;
        }
        Turn.NumCombatUndefeats = 0;
        Turn.NumCombatEvades = 0;
    }

    public override EventType Type =>
        EventType.OnPostHorde;
}

