using System;
using UnityEngine;

public class EventBeforeActRestrictCard : Event
{
    [Tooltip("does the effect affect the entire party?")]
    public bool ApplyToParty = true;
    [Tooltip("checks to roll")]
    public SkillCheckValueType[] Checks;
    [Tooltip("the effect duration")]
    public int Duration = 1;
    [Tooltip("message to be displayed during the check")]
    public StrRefType Message;
    [Tooltip("specifies the restricted card")]
    public CardSelector RestrictedCard;

    public override void OnBeforeAct()
    {
        if (this.ApplyToParty)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Effect e = new EffectCardRestrictionPending(Turn.Card.ID, this.Duration, this.Checks, this.Message.id, this.RestrictedCard.ToFilter());
                Party.Characters[i].ApplyEffect(e);
            }
        }
        else
        {
            Effect effect2 = new EffectCardRestrictionPending(Turn.Card.ID, this.Duration, this.Checks, this.Message.id, this.RestrictedCard.ToFilter());
            Turn.Owner.ApplyEffect(effect2);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardBeforeAct;
}

