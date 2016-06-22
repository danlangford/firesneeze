using System;
using UnityEngine;

public class LocationPowerFoxgloveHaunts : LocationPower
{
    [Tooltip("card selector describing aldren foxglove")]
    public CardSelector Foxglove;

    public override void Activate()
    {
        base.PowerBegin();
        base.ShowCancelButton(true);
        Turn.EmptyLayoutDecks = false;
        Turn.PushReturnState();
        Turn.PushCancelDestination(new TurnStateCallback(this, "LocationPowerFoxglove_Cancel"));
        CardFilter filter = this.Foxglove.ToFilter();
        TurnStateData data = new TurnStateData(ActionType.Reveal, filter, 1);
        Turn.SetStateData(data);
        Turn.PushStateDestination(new TurnStateCallback(this, "LocationPowerFoxglove_Finish"));
        Turn.State = GameStateType.Power;
    }

    public override bool IsValid()
    {
        if ((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish))
        {
            if ((this.Foxglove != null) && (this.Foxglove.Filter(Turn.Character.Hand) <= 0))
            {
                return false;
            }
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].GetEffect(EffectType.Haunt) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void LocationPowerFoxglove_Cancel()
    {
        this.PowerEnd();
        Turn.EmptyLayoutDecks = false;
        Turn.ReturnToReturnState();
        Turn.EmptyLayoutDecks = true;
    }

    private void LocationPowerFoxglove_Finish()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Effect e = Party.Characters[i].GetEffect(EffectType.Haunt);
            if (e != null)
            {
                Party.Characters[i].RemoveEffect(e);
            }
        }
        Turn.ReturnToReturnState();
        this.PowerEnd();
        Turn.EmptyLayoutDecks = true;
    }
}

