using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelStatus : GuiPanel
{
    [Tooltip("reference to the health bar in this scene")]
    public Animator HealthBar;
    [Tooltip("reference to the xp reward vfx in this scene")]
    public GameObject VfxRewardExperience;
    public GuiLabel VfxRewardExperienceLabel;

    public override void Initialize()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Deck.Changed += new Deck.EventHandlerDeckChanged(this.OnDeckChanged);
        }
    }

    private bool IsCharacterCriticalLowHealth(Character character) => 
        ((character.Deck.Count <= 2) && (Settings.GraphicsLevel > 0));

    private bool IsCharacterLowHealth(Character character) => 
        (character.Deck.Count <= character.HandSize);

    private void OnDeckChanged(object sender, EventArgsCard e)
    {
        this.Refresh();
        if (!base.Locked)
        {
            if (this.IsCharacterLowHealth(Turn.Character) && ((Turn.Character.Deck.Count - e.ChangeInSize) > Turn.Character.HandSize))
            {
                UI.Sound.Play(SoundEffectType.LowHealthDanger);
            }
            else if (this.IsCharacterCriticalLowHealth(Turn.Character) && ((Turn.Character.Deck.Count - e.ChangeInSize) > 2))
            {
                UI.Sound.Play(SoundEffectType.CriticalHealth);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].Deck.Changed -= new Deck.EventHandlerDeckChanged(this.OnDeckChanged);
        }
    }

    public override void Refresh()
    {
        if (!base.Locked)
        {
            if (this.IsCharacterCriticalLowHealth(Turn.Character))
            {
                this.HealthBar.Play("health_critical_idle");
            }
            else if (this.IsCharacterLowHealth(Turn.Character))
            {
                this.HealthBar.Play("health_low_idle");
            }
            else
            {
                this.HealthBar.Play("health_normal_idle");
            }
        }
    }

    public void ShowExperience(int amount)
    {
        if (!base.Locked)
        {
            base.StartCoroutine(this.ShowMessageXP(amount));
        }
    }

    public void ShowGold(int amount)
    {
        if (!base.Locked)
        {
            Game.UI.GoldPanel.Show(amount);
        }
    }

    [DebuggerHidden]
    private IEnumerator ShowMessageXP(int amount) => 
        new <ShowMessageXP>c__Iterator6F { 
            amount = amount,
            <$>amount = amount,
            <>f__this = this
        };

    [CompilerGenerated]
    private sealed class <ShowMessageXP>c__Iterator6F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>amount;
        internal GuiPanelStatus <>f__this;
        internal int amount;

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
                    this.<>f__this.VfxRewardExperience.SetActive(true);
                    this.<>f__this.VfxRewardExperienceLabel.Text = "+" + this.amount + " XP";
                    this.$current = new WaitForSeconds(2.4f);
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.VfxRewardExperience.SetActive(false);
                    this.$PC = -1;
                    break;
            }
            return false;
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

