using System;
using UnityEngine;

public class PowerModifierDifficulty : PowerModifier
{
    [Tooltip("if true this modifier will be applied to the party or just to the current character")]
    public bool ApplyToParty;
    [Tooltip("the selector needed to match for the difficulty modifier to take effect")]
    public CardSelector CardSelector;
    [Tooltip("which check to defeat to apply to. Negative means all checks to defeat.")]
    public int CheckSequence = -1;
    [Tooltip("when set to a value besides zero will modify the check by this amount")]
    public int DifficultyModifier;
    [Tooltip("the duration to apply the difficulty modifier")]
    public int Duration;

    public override void Activate(int powerIndex)
    {
        CardFilter empty;
        if (this.CardSelector != null)
        {
            if (this.CardSelector.CardIDs.Length < 1)
            {
                this.CardSelector.CardIDs = new string[1];
            }
            this.CardSelector.CardIDs[0] = Turn.Card.ID;
            empty = this.CardSelector.ToFilter();
        }
        else
        {
            empty = CardFilter.Empty;
        }
        EffectModifyDifficulty e = new EffectModifyDifficulty(Effect.GetEffectID(this), this.Duration, this.DifficultyModifier, SkillCheckType.None, empty, this.CheckSequence, false);
        if (this.ApplyToParty)
        {
            Party.ApplyEffect(e);
        }
        else
        {
            Turn.Character.ApplyEffect(e);
        }
    }

    public override void Deactivate()
    {
        if (this.ApplyToParty)
        {
            Party.RemoveEffect(Effect.GetEffectID(this));
        }
        else
        {
            Turn.Character.RemoveEffect(Effect.GetEffectID(this));
        }
        Turn.Validate = true;
    }
}

