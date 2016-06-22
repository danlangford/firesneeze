using System;
using UnityEngine;

public class PowerConditionChecks : PowerCondition
{
    [Tooltip("turn must have at least one of these checks")]
    public SkillCheckType[] Checks;
    [Tooltip("turn must be on this check sequence")]
    public int CheckSequence;
    [Tooltip("turn must be in the encounter stage (not before or after)")]
    public bool EncounterStage;
    [Tooltip("if true, only the participating checks will be considered")]
    public bool Strict = true;
    [Tooltip("turn must have all of these traits")]
    public TraitType[] Traits;

    public override bool Evaluate(Card card)
    {
        if (Turn.DamageTraits != null)
        {
            for (int i = 0; i < this.Traits.Length; i++)
            {
                bool flag = false;
                for (int j = 0; j < Turn.DamageTraits.Count; j++)
                {
                    if (this.Traits[i] == ((TraitType) Turn.DamageTraits[j]))
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
        }
        if (this.Checks.Length > 0)
        {
            bool flag2 = false;
            for (int k = 0; k < this.Checks.Length; k++)
            {
                if (Rules.IsSkillParticipatingInCheck(this.Checks[k], !this.Strict))
                {
                    flag2 = true;
                    break;
                }
            }
            if (!flag2)
            {
                return false;
            }
        }
        if ((this.CheckSequence != 0) && (Turn.CombatCheckSequence != this.CheckSequence))
        {
            return false;
        }
        if (this.EncounterStage && (Turn.CombatStage != TurnCombatStageType.Encounter))
        {
            return false;
        }
        return true;
    }
}

