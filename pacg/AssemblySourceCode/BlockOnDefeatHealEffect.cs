using System;
using UnityEngine;

public class BlockOnDefeatHealEffect : BlockEffect
{
    [Tooltip("Amount of cards to heal.")]
    public int HealAmount = 1;
    [Tooltip("Which deck to heal from.")]
    public DeckType HealFrom = DeckType.Discard;
    [Tooltip("Target to heal.")]
    public int HealTarget;
    [Tooltip("If true the effect will heal the current turn owner.")]
    public bool HealTargetIsOwner = true;
    [Tooltip("Where to heal cards to.")]
    public DeckPositionType Position = DeckPositionType.Shuffle;

    protected override Effect CreateEffect(string source, int duration, CardFilter filter)
    {
        if (this.HealTargetIsOwner)
        {
            this.HealTarget = Turn.Current;
        }
        return new EffectOnDefeatHeal(source, duration, filter, this.HealTarget, this.HealAmount, this.HealFrom, this.Position);
    }
}

