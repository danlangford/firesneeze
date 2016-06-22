using System;
using UnityEngine;

public class RandomizedPowersDiceModel : AutoDiceModel
{
    [Tooltip("all the power labels that need to be set")]
    public GuiLabel[] powerLabels;

    protected override void PlayRollAnimation(Vector2 direction)
    {
        base.PlayRollAnimation(direction);
        base.backgroundAnimator.SetTrigger("Roll");
    }

    protected override void RotateToSide(int side)
    {
        base.RotateToSide(side);
        base.backgroundAnimator.SetInteger("ResultInt", side);
    }
}

