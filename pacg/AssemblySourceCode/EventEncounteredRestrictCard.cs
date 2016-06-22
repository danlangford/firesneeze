using System;
using UnityEngine;

public class EventEncounteredRestrictCard : Event
{
    [Tooltip("does the effect affect the entire party?")]
    public bool ApplyToParty = true;
    [Tooltip("the effect duration")]
    public int Duration = 1;
    [Tooltip("specifies the restricted card")]
    public CardSelector RestrictedCard;

    public override void OnCardEncountered(Card card)
    {
        if (!base.IsConditionValid(card))
        {
            Event.Done();
        }
        else
        {
            CardFilter filter = this.RestrictedCard.ToFilter();
            EffectCardRestriction e = new EffectCardRestriction(card.ID, this.Duration, filter);
            if (this.ApplyToParty)
            {
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    Party.Characters[i].ApplyEffect(e);
                }
            }
            else
            {
                Turn.Character.ApplyEffect(e);
            }
            Event.Done();
        }
    }

    public override EventType Type =>
        EventType.OnCardEncountered;
}

