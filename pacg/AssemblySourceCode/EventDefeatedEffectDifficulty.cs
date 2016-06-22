using System;
using UnityEngine;

public class EventDefeatedEffectDifficulty : Event
{
    [Tooltip("the amount to change the difficulty")]
    public int Amount = 1;
    [Tooltip("Negative means apply to all checks to defeat.")]
    public int CheckSequence = -1;
    [Tooltip("should we restrict this effect to only check to defeat/acquire")]
    public bool CombatOnly;
    [Tooltip("number of turns")]
    public int Duration = 1;
    [Tooltip("determines when the effect is enforced")]
    public CardSelector Selector;

    public override void OnCardDefeated(Card card)
    {
        if (this.Selector != null)
        {
            EffectModifyDifficulty e = new EffectModifyDifficulty(card.ID, this.Duration, this.Amount, SkillCheckType.None, this.Selector.ToFilter(), this.CheckSequence, this.CombatOnly);
            Party.ApplyEffect(e);
        }
        Event.Done();
    }

    public override EventType Type =>
        EventType.OnCardDefeated;
}

