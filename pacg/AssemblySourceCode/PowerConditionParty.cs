using System;
using UnityEngine;

public class PowerConditionParty : PowerCondition
{
    [Tooltip("at least one person in the party is haunted")]
    public bool Haunted;

    public override bool Evaluate(Card card)
    {
        if (this.Haunted)
        {
            bool flag = false;
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (Party.Characters[i].GetEffect(EffectType.Haunt) != null)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
        }
        return true;
    }
}

