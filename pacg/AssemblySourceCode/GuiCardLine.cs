using System;
using UnityEngine;

public class GuiCardLine : GuiElement
{
    [Tooltip("reference to the animator in this object")]
    public UnityEngine.Animator Animator;
    [Tooltip("reference to the card rank label in this object")]
    public GuiLabel CardRankLabel;
    [Tooltip("reference to the card type label in this object")]
    public GuiLabel CardTypeLabel;
    [Tooltip("reference to the four sprite counters in this object")]
    public SpriteRenderer[] Counters;
    [Tooltip("sprites which represent numbers 0 through 10")]
    public Sprite[] CounterSprites;
    private CardType myCardType;
    [Tooltip("reference to the levelup button in this object")]
    public GuiButton UpgradeButton;
    [Tooltip("sprite which represents the upgrade potential")]
    public SpriteRenderer Upgrades;
    [Tooltip("sprites which represent upgrade sequences")]
    public Sprite[] UpgradeSprites;

    private void RefreshCounters(int min, int current, int max)
    {
        int num = current - min;
        for (int i = 0; i < this.Counters.Length; i++)
        {
            int index = i + min;
            if ((i < num) && (index < this.CounterSprites.Length))
            {
                this.Counters[i].sprite = this.CounterSprites[index];
            }
        }
    }

    private void RefreshUpgrades(int min, int current, int max)
    {
        int index = max - min;
        if ((index >= 0) && (index < this.UpgradeSprites.Length))
        {
            this.Upgrades.sprite = this.UpgradeSprites[index];
        }
        else
        {
            this.Upgrades.sprite = null;
        }
    }

    public void Select(int min, int current, int max)
    {
        int num = current - min;
        this.UpgradeButton.Glow(true);
        this.Animator.SetInteger("SkillNum", num);
        this.Animator.SetTrigger("AddSkillPoint");
        this.CardRankLabel.Text = current.ToString();
        this.RefreshCounters(min, current, max);
    }

    public void SetRank(int min, int current, int max)
    {
        this.CardRankLabel.Text = current.ToString();
        string stateName = string.Empty;
        int num = current - min;
        if (num >= 4)
        {
            stateName = "cardupgrade_idle4";
        }
        if (num == 3)
        {
            stateName = "cardupgrade_idle3";
        }
        if (num == 2)
        {
            stateName = "cardupgrade_idle2";
        }
        if (num == 1)
        {
            stateName = "cardupgrade_idle1";
        }
        if (num <= 0)
        {
            stateName = "cardupgrade_idle0";
        }
        this.Animator.Play(stateName);
        this.RefreshCounters(min, current, max);
        this.RefreshUpgrades(min, current, max);
    }

    public void Unselect(int min, int current, int max)
    {
        int num = current - min;
        this.UpgradeButton.Glow(false);
        this.Animator.SetInteger("SkillNum", num);
        this.Animator.SetTrigger("Reverse");
        this.CardRankLabel.Text = current.ToString();
        this.RefreshCounters(min, current, max);
    }

    public CardType Type
    {
        get => 
            this.myCardType;
        set
        {
            this.CardTypeLabel.Text = value.ToText().ToUpper();
            this.myCardType = value;
        }
    }
}

