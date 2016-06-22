using System;
using UnityEngine;

public class AutoDiceModel : DiceModel
{
    protected Animator backgroundAnimator;

    protected override void Awake()
    {
        base.Awake();
        if (base.anim == null)
        {
            base.anim = base.transform.GetChild(0).GetComponentInChildren<Animator>();
        }
        base.model = base.transform.GetChild(0);
        this.backgroundAnimator = base.GetComponent<Animator>();
        this.Fade(true, 0f);
    }

    public override void Fade(bool isVisible, float time)
    {
        if (isVisible)
        {
            this.backgroundAnimator.SetTrigger("Start");
        }
        else
        {
            this.backgroundAnimator.SetTrigger("End");
        }
    }

    protected override void PlayIdleAnimation()
    {
        this.RotateToSide(base.Side);
        base.anim.ResetTrigger("Reset");
        base.anim.SetTrigger("SpinStop");
    }

    protected override void PlayRollAnimation(Vector2 direction)
    {
        base.anim.ResetTrigger("SpinStop");
        base.anim.SetTrigger("Reset");
        base.anim.SetTrigger("Spin");
        this.backgroundAnimator.SetTrigger("End");
    }

    protected override void SetupRotationMatrix()
    {
        if (base.diceType == DiceType.D4)
        {
            base.rotations = new Quaternion[] { new Quaternion(0.1797528f, -0.7595908f, -0.624561f, 0.02518816f), new Quaternion(-0.628207f, -0.4684723f, -0.2454085f, 0.5706701f), new Quaternion(-0.7266016f, 0.3749801f, 0.165935f, 0.5512764f), Quaternion.identity };
        }
        else if (base.diceType == DiceType.D6)
        {
            base.rotations = new Quaternion[] { Quaternion.identity, new Quaternion(-0.5907699f, -0.3995816f, -0.3927083f, 0.580608f), new Quaternion(-0.7001773f, 0.1357438f, 0.1334088f, 0.6881334f), new Quaternion(0.6869617f, -0.1248692f, 0.1362224f, 0.702805f), new Quaternion(0.5914128f, 0.3985822f, -0.3861941f, 0.5849935f), new Quaternion(-0.8285444f, -0.559787f, -0.006254624f, -0.01065744f) };
        }
        else if (base.diceType == DiceType.D12)
        {
            base.rotations = new Quaternion[] { new Quaternion(-0.1300645f, -0.4277326f, 0.628999f, -0.6380633f), new Quaternion(0.4153361f, -0.7675183f, 0.374805f, -0.3129423f), new Quaternion(0.3893239f, -0.7366316f, -0.2499221f, -0.4932949f), new Quaternion(0.2901393f, -0.4282909f, -0.7654766f, -0.3826638f), new Quaternion(0f, 0f, 0.9699482f, 0.2433116f), new Quaternion(-0.2462627f, 0.4924722f, -0.8265172f, 0.117026f), new Quaternion(-0.5760744f, -0.5889276f, -0.5631238f, 0.06476163f), new Quaternion(-0.5555364f, -0.6399806f, -0.1010999f, 0.5211363f), new Quaternion(-0.09405489f, -0.5356492f, -0.1733497f, 0.8210868f), new Quaternion(0.580735f, -0.1939785f, -0.1679331f, 0.8222117f), new Quaternion(-0.850598f, -0.1387343f, 0.0998738f, -0.4972535f), new Quaternion(0.8748041f, 0.4838104f, 0.006757761f, -0.02449056f) };
        }
        else
        {
            base.SetupRotationMatrix();
        }
    }

    protected override bool DisablesAnimator =>
        false;
}

