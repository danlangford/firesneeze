using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RewardIconExperience : MonoBehaviour
{
    [Tooltip("reference to the xp bar image in our hierarchy")]
    public GameObject Bar;
    [Tooltip("reference to the xp bar animator in our hierarchy")]
    public Animator BarAnimator;
    [Tooltip("reference to the xp ratio label in our hierarchy")]
    public GuiLabel ExperienceLabel;
    [Tooltip("reference to the level label in our hierarchy")]
    public GuiLabel LevelLabel;
    private Character myCharacter;
    [Tooltip("reference to the portrait sprite renderer in our hierarchy")]
    public SpriteRenderer PortraitRenderer;
    [Tooltip("reference to the description label in our hierarchy")]
    public GuiLabel RewardsLabel;
    [Tooltip("reference to the sparks at the right hand side of the bar")]
    public GameObject Sparks;

    private float GetBarScale()
    {
        int num = Rules.GetExperiencePointsForLevel(this.Character.Level + 1) - Rules.GetExperiencePointsForLevel(this.Character.Level);
        int num2 = this.Character.XP - Rules.GetExperiencePointsForLevel(this.Character.Level);
        float num3 = 1f;
        if (num2 > 0)
        {
            num3 = Mathf.Clamp01(((float) num2) / ((float) num));
        }
        return num3;
    }

    public void Levelup()
    {
        base.StartCoroutine(this.LevelupCoroutine(this.Character.XPX));
    }

    [DebuggerHidden]
    private IEnumerator LevelupCoroutine(int xp) => 
        new <LevelupCoroutine>c__Iterator9F { 
            xp = xp,
            <$>xp = xp,
            <>f__this = this
        };

    private void OnLevelup(int level)
    {
        if (((level >= 0) && (level < Game.Rewards.Rewards.Length)) && !string.IsNullOrEmpty(Game.Rewards.Rewards[level]))
        {
            int index = Party.IndexOf(this.Character.ID);
            Reward reward = this.Owner.QueueReward(index, Game.Rewards.Rewards[level]);
            if (reward != null)
            {
                string text = this.RewardsLabel.Text;
                object[] objArray1 = new object[] { text, "+ ", reward.Description, Environment.NewLine };
                this.RewardsLabel.Text = string.Concat(objArray1);
            }
        }
    }

    private void Refresh()
    {
        int experiencePointsForLevel = Rules.GetExperiencePointsForLevel(this.Character.Level + 1);
        int num2 = Rules.GetExperiencePointsForLevel(this.Character.Level + 1) - Rules.GetExperiencePointsForLevel(this.Character.Level);
        int num3 = this.Character.XP - Rules.GetExperiencePointsForLevel(this.Character.Level);
        float x = Mathf.Clamp01(((float) num3) / ((float) num2));
        this.Bar.transform.localScale = new Vector3(x, this.Bar.transform.localScale.y, this.Bar.transform.localScale.z);
        object[] objArray1 = new object[] { this.Character.XP, " / ", experiencePointsForLevel, " XP" };
        this.ExperienceLabel.Text = string.Concat(objArray1);
        this.LevelLabel.Text = this.Character.Level.ToString();
    }

    public Character Character
    {
        get => 
            this.myCharacter;
        set
        {
            this.myCharacter = value;
            if (this.myCharacter != null)
            {
                this.PortraitRenderer.sprite = this.myCharacter.PortraitAvatar;
                this.RewardsLabel.Text = string.Empty;
                this.Refresh();
            }
        }
    }

    public GuiPanelLevelup Owner { get; set; }

    [CompilerGenerated]
    private sealed class <LevelupCoroutine>c__Iterator9F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>xp;
        internal RewardIconExperience <>f__this;
        internal int <amount>__3;
        internal bool <isLevelGained>__0;
        internal int <max>__2;
        internal int <next>__1;
        internal float <scale>__4;
        internal int xp;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<isLevelGained>__0 = false;
                    this.<>f__this.BarAnimator.SetTrigger("Start");
                    this.<>f__this.Sparks.SetActive(false);
                    break;

                case 1:
                    if (this.<amount>__3 < this.<max>__2)
                    {
                        break;
                    }
                    this.<>f__this.BarAnimator.SetTrigger("Levelup");
                    this.<>f__this.Sparks.SetActive(true);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 2;
                    goto Label_02A9;

                case 2:
                    this.<>f__this.Sparks.SetActive(false);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 3;
                    goto Label_02A9;

                case 3:
                    this.<isLevelGained>__0 = true;
                    this.<>f__this.Refresh();
                    break;

                case 4:
                    goto Label_0240;

                default:
                    goto Label_02A7;
            }
            if (this.xp > 0)
            {
                this.<next>__1 = Rules.GetExperiencePointsForLevel(this.<>f__this.Character.Level + 1);
                if (this.<next>__1 > 0)
                {
                    this.<max>__2 = this.<next>__1 - this.<>f__this.Character.XP;
                    this.<amount>__3 = Mathf.Min(this.xp, this.<max>__2);
                    this.xp -= this.<amount>__3;
                    Character character = this.<>f__this.Character;
                    character.XP += this.<amount>__3;
                    if (this.<amount>__3 >= this.<max>__2)
                    {
                        this.<>f__this.OnLevelup(this.<>f__this.Character.Level);
                    }
                    this.<scale>__4 = this.<>f__this.GetBarScale();
                    LeanTween.scaleX(this.<>f__this.Bar, this.<scale>__4, 0.75f);
                    this.$current = new WaitForSeconds(0.75f);
                    this.$PC = 1;
                    goto Label_02A9;
                }
            }
            if (!this.<isLevelGained>__0)
            {
                this.<>f__this.BarAnimator.SetTrigger("NoLevelup");
                this.$current = new WaitForSeconds(0.5f);
                this.$PC = 4;
                goto Label_02A9;
            }
        Label_0240:
            this.<>f__this.Character.XPX = 0;
            if (string.IsNullOrEmpty(this.<>f__this.RewardsLabel.Text))
            {
                this.<>f__this.RewardsLabel.Text = "No Rewards";
            }
            this.<>f__this.Refresh();
            this.<>f__this.BarAnimator.SetTrigger("End");
            this.$PC = -1;
        Label_02A7:
            return false;
        Label_02A9:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current =>
            this.$current;

        object IEnumerator.Current =>
            this.$current;
    }
}

