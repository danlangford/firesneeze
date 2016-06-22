using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Reward : MonoBehaviour
{
    [Tooltip("text displayed on the levelup screen")]
    public StrRefType Description;
    [Tooltip("the unique ID for this reward; the characters track rewards received")]
    public string ID;
    protected bool isPanelShowing;
    [Tooltip("text displayed at screen bottom when showing this reward")]
    public StrRefType Message;
    protected GameObject myPanel;
    protected GuiWindowReward myWindow;
    protected int rewardNumber;

    protected Reward()
    {
    }

    public virtual void Deliver()
    {
    }

    public virtual void Display()
    {
        UI.Busy = false;
        this.Locked = false;
        this.Refresh();
    }

    protected virtual void Generate()
    {
    }

    public virtual bool GetAllRewardsGiven() => 
        true;

    public virtual float GetInitializationTime() => 
        0f;

    public virtual int GetNumRewards(Character character) => 
        1;

    protected GameObject GetRewardPanel(string panelName)
    {
        GameObject obj2 = GameObject.Find("/Animations");
        if (obj2 != null)
        {
            for (int i = 0; i < obj2.transform.childCount; i++)
            {
                Transform child = obj2.transform.GetChild(i);
                if ((child != null) && (child.name == panelName))
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    protected virtual string GetRewardPanelName() => 
        null;

    public virtual float GetShowTime() => 
        0f;

    protected virtual float GetStartTime() => 
        1.1f;

    public virtual bool HasReward(int n) => 
        false;

    public virtual void Initialize(GuiWindowReward window)
    {
        this.myWindow = window;
        this.myPanel = this.GetRewardPanel(this.GetRewardPanelName());
    }

    public virtual bool IsSelected(int n) => 
        false;

    protected void Lock(float duration)
    {
        this.Locked = true;
        LeanTween.delayedCall(duration, (Action) (() => (this.Locked = false)));
    }

    public virtual void Pause(bool isPaused)
    {
    }

    protected void PlayPanelAnimation(string name)
    {
        if (this.myPanel != null)
        {
            Animator component = this.myPanel.GetComponent<Animator>();
            if ((component == null) && (this.myPanel.transform.childCount >= 1))
            {
                component = this.myPanel.transform.GetChild(0).GetComponent<Animator>();
            }
            if (component != null)
            {
                component.SetTrigger(name);
            }
        }
    }

    public virtual void Refresh()
    {
    }

    public virtual void Select(AttributeType attribute)
    {
    }

    public virtual void Select(Card card)
    {
    }

    public virtual void Select(CardType card)
    {
    }

    public virtual void Select(ProficencyType proficiency)
    {
    }

    public virtual void Select(RoleTableEntry role)
    {
    }

    public virtual void Select(int handSizeDelta)
    {
    }

    public virtual void Select(string power)
    {
    }

    public void SetReward(int n)
    {
        this.rewardNumber = n;
    }

    public virtual void Show(bool isVisible)
    {
        if (isVisible)
        {
            this.Generate();
        }
        if (this.myPanel != null)
        {
            this.myPanel.SetActive(isVisible);
        }
        if ((isVisible && !this.isPanelShowing) && this.Animated)
        {
            UI.Busy = true;
            this.PlayPanelAnimation("Start");
            LeanTween.delayedCall(this.GetStartTime(), new Action(this.Display));
        }
        this.isPanelShowing = isVisible;
    }

    protected virtual bool Animated =>
        true;

    protected bool Locked { get; set; }

    public virtual bool Player =>
        false;

    public virtual int Priority =>
        0;
}

