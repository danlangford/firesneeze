using System;
using UnityEngine;

public class DiceModel : MonoBehaviour
{
    protected Animator anim;
    private float currentFrame;
    private int currentHash;
    [Tooltip("the type of dice")]
    public DiceType diceType;
    private static int[] lastAnim;
    protected Transform model;
    protected Quaternion[] rotations;
    private int side;

    protected virtual void Awake()
    {
        this.anim = base.transform.GetChild(0).GetComponent<Animator>();
        this.model = base.transform.GetChild(0).GetChild(0);
        this.side = this.GetStartingSide();
        this.Skew(15);
        this.SetupRotationMatrix();
        lastAnim = new int[Constants.NUM_DICE_TYPES];
    }

    public virtual void Fade(bool isVisible, float time)
    {
        float to = !isVisible ? 0f : 1f;
        LeanTween.alpha(this.model.gameObject, to, time);
    }

    private int GetStartingSide()
    {
        if (this.diceType == DiceType.D4)
        {
            return 4;
        }
        if (this.diceType == DiceType.D6)
        {
            return 6;
        }
        if (this.diceType == DiceType.D8)
        {
            return 8;
        }
        if (this.diceType == DiceType.D10)
        {
            return 10;
        }
        if (this.diceType == DiceType.D12)
        {
            return 12;
        }
        return 1;
    }

    public void Pause(bool isPaused)
    {
        if (isPaused)
        {
            AnimatorStateInfo currentAnimatorStateInfo = this.anim.GetCurrentAnimatorStateInfo(0);
            this.currentFrame = currentAnimatorStateInfo.normalizedTime;
            this.currentHash = currentAnimatorStateInfo.fullPathHash;
        }
        else
        {
            LeanTween.delayedCall(0.001f, () => this.anim.Play(this.currentHash, -1, this.currentFrame));
        }
    }

    protected virtual void PlayIdleAnimation()
    {
        string stateName = null;
        if (this.diceType == DiceType.D4)
        {
            stateName = "d4_idle";
        }
        if (this.diceType == DiceType.D6)
        {
            stateName = "d6_idle";
        }
        if (this.diceType == DiceType.D8)
        {
            stateName = "d8_idle";
        }
        if (this.diceType == DiceType.D10)
        {
            stateName = "d10_idle";
        }
        if (this.diceType == DiceType.D12)
        {
            stateName = "d12_idle";
        }
        if (stateName != null)
        {
            this.anim.CrossFade(stateName, 0.1f, -1, 0f);
        }
    }

    protected virtual void PlayRollAnimation(Vector2 direction)
    {
        this.RotateToSide(this.Side);
        int num = UnityEngine.Random.Range(1, 4);
        if ((num == lastAnim[(int) this.diceType]) && (++num >= 4))
        {
            num = 1;
        }
        lastAnim[(int) this.diceType] = num;
        string stateName = null;
        if (this.diceType == DiceType.D4)
        {
            stateName = "d4_roll0" + num;
        }
        if (this.diceType == DiceType.D6)
        {
            stateName = "d6_roll0" + num;
        }
        if (this.diceType == DiceType.D8)
        {
            stateName = "d8_roll0" + num;
        }
        if (this.diceType == DiceType.D10)
        {
            stateName = "d10_roll0" + num;
        }
        if (this.diceType == DiceType.D12)
        {
            stateName = "d12_roll0" + num;
        }
        if (stateName != null)
        {
            this.anim.CrossFade(stateName, 0.1f, -1, 0f);
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * 57.29578f;
        base.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public virtual void Reset()
    {
        Renderer component = this.model.GetComponent<Renderer>();
        if (component != null)
        {
            Color color = component.material.color;
            if (color.a < 1f)
            {
                component.material.color = new Color(color.r, color.g, color.b, 1f);
            }
        }
    }

    public void Roll(bool isRolling, Vector2 direction)
    {
        if (this.DisablesAnimator)
        {
            this.anim.enabled = isRolling;
        }
        if (isRolling)
        {
            this.PlayRollAnimation(direction);
        }
        else
        {
            this.PlayIdleAnimation();
        }
    }

    protected virtual void RotateToSide(int side)
    {
        if ((this.rotations != null) && (this.rotations.Length >= 1))
        {
            if (side > this.rotations.Length)
            {
                side = 1;
            }
            this.model.rotation = this.rotations[side - 1];
        }
    }

    protected virtual void SetupRotationMatrix()
    {
        if (this.diceType == DiceType.D4)
        {
            this.rotations = new Quaternion[] { Quaternion.Euler(new Vector3(70f, 145f, 90f)), Quaternion.Euler(new Vector3(45f, 240f, 65f)), Quaternion.Euler(new Vector3(0f, 250f, 325f)), Quaternion.Euler(new Vector3(0f, 0f, 0f)) };
        }
        if (this.diceType == DiceType.D6)
        {
            this.rotations = new Quaternion[] { Quaternion.Euler(new Vector3(0f, 180f, 180f)), Quaternion.Euler(new Vector3(90f, -90f, -90f)), Quaternion.Euler(new Vector3(90f, 90f, 0f)), Quaternion.Euler(new Vector3(-90f, -90f, 0f)), Quaternion.Euler(new Vector3(-90f, -90f, 90f)), Quaternion.Euler(new Vector3(0f, 0f, 0f)) };
        }
        if (this.diceType == DiceType.D8)
        {
            this.rotations = new Quaternion[] { Quaternion.Euler(new Vector3(30f, 80f, 190f)), Quaternion.Euler(new Vector3(345f, 250f, 345f)), Quaternion.Euler(new Vector3(0f, 175f, 45f)), Quaternion.Euler(new Vector3(330f, 130f, 250f)), Quaternion.Euler(new Vector3(60f, 350f, 240f)), Quaternion.Euler(new Vector3(50f, 100f, 25f)), Quaternion.Euler(new Vector3(55f, 350f, 350f)), Quaternion.Euler(new Vector3(340f, 0f, 180f)) };
        }
        if (this.diceType == DiceType.D10)
        {
            this.rotations = new Quaternion[] { Quaternion.Euler(new Vector3(20f, 230f, 150f)), Quaternion.Euler(new Vector3(40f, 90f, 220f)), Quaternion.Euler(new Vector3(35f, 275f, 280f)), Quaternion.Euler(new Vector3(35f, 50f, 300f)), Quaternion.Euler(new Vector3(335f, 215f, 345f)), Quaternion.Euler(new Vector3(310f, 85f, 280f)), Quaternion.Euler(new Vector3(50f, 300f, 230f)), Quaternion.Euler(new Vector3(0f, 295f, 350f)), Quaternion.Euler(new Vector3(360f, 175f, 170f)), Quaternion.Euler(new Vector3(5f, 10f, 30f)) };
        }
        if (this.diceType == DiceType.D12)
        {
            this.rotations = new Quaternion[] { Quaternion.Euler(new Vector3(335f, 225f, 185f)), Quaternion.Euler(new Vector3(-30f, -60f, 175f)), Quaternion.Euler(new Vector3(315f, 290f, 100f)), Quaternion.Euler(new Vector3(300f, 190f, 160f)), Quaternion.Euler(new Vector3(0f, 180f, 0f)), Quaternion.Euler(new Vector3(345f, 245f, 245f)), Quaternion.Euler(new Vector3(55f, 335f, 50f)), Quaternion.Euler(new Vector3(40f, 20f, 40f)), Quaternion.Euler(new Vector3(0f, 115f, 120f)), Quaternion.Euler(new Vector3(300f, 160f, 120f)), Quaternion.Euler(new Vector3(-45f, -45f, 20f)), Quaternion.Euler(new Vector3(0f, 0f, 0f)) };
        }
    }

    private void Skew(int amount)
    {
        int num = UnityEngine.Random.Range(-amount, amount);
        this.model.Rotate(0f, 0f, (float) num);
    }

    protected virtual bool DisablesAnimator =>
        true;

    public int Side
    {
        get => 
            this.side;
        set
        {
            this.side = value;
        }
    }
}

