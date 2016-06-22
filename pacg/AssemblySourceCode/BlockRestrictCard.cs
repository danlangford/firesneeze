using System;
using UnityEngine;

public class BlockRestrictCard : Block
{
    [Tooltip("apply to all characters or just the current")]
    public bool ApplyToAllCharacters = true;
    [Tooltip("the card to restrict")]
    public CardSelector RestrictedCard;
    [Tooltip("the duration of turns to restrict for")]
    public int RestrictionDuration = 1;

    public override void Invoke()
    {
        CardFilter filter = this.RestrictedCard.ToFilter();
        EffectCardRestriction e = new EffectCardRestriction(Effect.GetEffectID(this), this.RestrictionDuration, filter);
        if (this.ApplyToAllCharacters)
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
    }
}

