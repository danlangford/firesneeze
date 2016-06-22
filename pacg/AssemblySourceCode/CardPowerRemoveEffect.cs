using System;
using UnityEngine;

public class CardPowerRemoveEffect : CardPower
{
    [Tooltip("remove haunts?")]
    public bool Haunted = true;

    public override void Activate(Card card)
    {
        if (this.Haunted)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Effect e = Party.Characters[i].GetEffect(EffectType.Haunt);
                if (e != null)
                {
                    Party.Characters[i].RemoveEffect(e);
                }
            }
        }
    }

    protected override bool IsPowerAllowed(Card card)
    {
        if (base.IsConditionValid(card))
        {
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
}

