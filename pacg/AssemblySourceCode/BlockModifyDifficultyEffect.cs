using System;
using UnityEngine;

public class BlockModifyDifficultyEffect : BlockEffect
{
    [Tooltip("the amount to change the difficulty")]
    public int Amount = 1;
    [Tooltip("the amount this effect is the dice total")]
    public bool AmountIsDiceTotal;
    [Tooltip("which check to defeat is this effect valid for. Negative means all checks.")]
    public int CheckSequence = -1;
    [Tooltip("restrict the power to only check to defeat/acquire")]
    public bool CombatOnly;
    [Tooltip("this effect automatically only applies to the current check sequence")]
    public bool CurrentCheckSequence;
    [Tooltip("make the card filter only apply to the Turn.Card")]
    public bool OnlyApplyToTurnCard;
    [Tooltip("valid checks to apply this effect")]
    public SkillCheckType SkillChecks;

    protected override Effect CreateEffect(string source, int duration, CardFilter filter)
    {
        if (this.OnlyApplyToTurnCard)
        {
            string[] strArray = new string[filter.CardIDs.Length + 1];
            for (int i = 0; i < filter.CardIDs.Length; i++)
            {
                strArray[i] = filter.CardIDs[i];
            }
            strArray[strArray.Length - 1] = Turn.Card.ID;
            filter.CardIDs = strArray;
        }
        int checkSequence = this.CheckSequence;
        if (this.CurrentCheckSequence)
        {
            checkSequence = Turn.CombatCheckSequence;
        }
        if (this.AmountIsDiceTotal)
        {
            return new EffectModifyDifficulty(source, duration, Turn.DiceTotal, this.SkillChecks, filter, checkSequence, this.CombatOnly);
        }
        return new EffectModifyDifficulty(source, duration, this.Amount, this.SkillChecks, filter, checkSequence, this.CombatOnly);
    }
}

