using System;
using UnityEngine;

public class BlockModifySkill : Block
{
    [Tooltip("Amount to modify the checks after failing the check")]
    public int ModifierAmount;
    [Tooltip("Number of turns to apply this penalty modifier")]
    public int ModifierDuration = 1;
    [Tooltip("the effect applies to this type of skill check to modify")]
    public SkillCheckType Skill;

    public override void Invoke()
    {
        Effect e = new EffectModifySkill(Effect.GetEffectID(this), this.ModifierDuration, this.Skill, this.ModifierAmount);
        Turn.Character.ApplyEffect(e);
        Turn.State = GameStateType.Combat;
    }

    public override bool Stateless =>
        false;
}

