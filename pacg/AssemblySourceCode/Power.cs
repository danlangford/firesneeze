using System;
using UnityEngine;

public abstract class Power : MonoBehaviour
{
    [Tooltip("can this power be cancelled?")]
    public bool CanCancel;
    [Tooltip("can only use this power when this condition is true")]
    public PowerConditionType[] Conditions;
    [Tooltip("when can this power be used again? default is once-per-check")]
    public PowerCooldownType Cooldown = PowerCooldownType.Check;
    [Tooltip("icon displayed on the button when power can be used")]
    public Sprite Icon;
    [Tooltip("icon displayed on the button when power cannot be used")]
    public Sprite IconDisabled;
    [Tooltip("icon displayed on the button when power is clicked")]
    public Sprite IconHilite;
    [Tooltip("unique ID for this power (used to save/load)")]
    public string ID;
    [Tooltip("only non-passive powers are displayed on buttons")]
    public bool Passive;

    protected Power()
    {
    }

    public virtual void Activate()
    {
        Turn.MarkPowerActive(this, true);
        if (this.Cancellable)
        {
            this.ShowCancelButton(true);
        }
    }

    public virtual void Deactivate()
    {
        Turn.MarkPowerActive(this, false);
    }

    protected virtual void GlowText(bool isGlowing)
    {
    }

    protected virtual void GlowText(bool isGlowing, float time)
    {
    }

    protected bool IsConditionValid(Card card) => 
        PowerCondition.Evaluate(card, this.Conditions);

    public virtual bool IsModifierActive(int n) => 
        false;

    public virtual bool IsValid() => 
        false;

    public virtual void OnDiceRolled()
    {
    }

    public virtual void OnExamineComplete()
    {
    }

    protected void PowerBegin()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.OnPowerActivated(this);
            this.GlowText(true);
        }
    }

    protected void PowerBegin(float time)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.OnPowerActivated(this);
            this.GlowText(true, time);
        }
    }

    protected virtual void PowerEnd()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.powersPanel.OnPowerDeactivated(this);
            this.GlowText(false);
        }
    }

    public virtual bool SetModifierActive(int n, bool active) => 
        false;

    protected void ShowCancelButton(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowCancelButton(isVisible);
        }
    }

    protected void ShowDice(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(isVisible);
        }
    }

    public virtual bool Cancellable =>
        this.CanCancel;

    public virtual string Description =>
        null;

    public virtual string Name =>
        null;
}

