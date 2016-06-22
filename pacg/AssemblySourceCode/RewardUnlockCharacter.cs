using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RewardUnlockCharacter : Reward
{
    [Tooltip("list of character IDs to unlock")]
    public string[] Characters;

    public override void Deliver()
    {
        for (int i = 0; i < this.Characters.Length; i++)
        {
            LicenseManager.GrantLicense(Constants.IAP_LICENSE_CH_PREFIX + this.Characters[i]);
        }
    }

    private GameObject GetRewardPanel(GameObject root, string panelName)
    {
        if (root != null)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform child = root.transform.GetChild(i);
                if ((child != null) && (child.name == panelName))
                {
                    return child.gameObject;
                }
            }
        }
        return null;
    }

    public override float GetShowTime() => 
        11f;

    public override bool HasReward(int n)
    {
        for (int i = 0; i < this.Characters.Length; i++)
        {
            if (!LicenseManager.GetIsLicensed(Constants.IAP_LICENSE_CH_PREFIX + this.Characters[i]))
            {
                return false;
            }
        }
        return true;
    }

    public override bool IsSelected(int n) => 
        base.isPanelShowing;

    public override void Show(bool isVisible)
    {
        if (isVisible)
        {
            base.StartCoroutine(this.ShowRewardSequence());
        }
        base.isPanelShowing = isVisible;
    }

    [DebuggerHidden]
    private IEnumerator ShowRewardSequence() => 
        new <ShowRewardSequence>c__IteratorA1 { <>f__this = this };

    public override bool Player =>
        true;

    [CompilerGenerated]
    private sealed class <ShowRewardSequence>c__IteratorA1 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal RewardUnlockCharacter <>f__this;
        internal GameObject <myRoot>__0;

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
                    UI.Busy = true;
                    this.<myRoot>__0 = this.<>f__this.GetRewardPanel("Reward_Prefab_UnlockCharacter");
                    this.<myRoot>__0.SetActive(true);
                    this.<>f__this.myPanel = this.<>f__this.GetRewardPanel(this.<myRoot>__0, "Unlock Character Anim");
                    this.<>f__this.myPanel.SetActive(true);
                    this.<>f__this.PlayPanelAnimation("Start");
                    UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
                    this.$current = new WaitForSeconds(2.5f);
                    this.$PC = 1;
                    goto Label_0188;

                case 1:
                    UI.Sound.Play(SoundEffectType.RewardCharacterFlip);
                    this.$current = new WaitForSeconds(3f);
                    this.$PC = 2;
                    goto Label_0188;

                case 2:
                    this.<>f__this.myPanel = this.<>f__this.GetRewardPanel(this.<myRoot>__0, "Unlock Character Anim 2");
                    this.<>f__this.myPanel.SetActive(true);
                    this.<>f__this.PlayPanelAnimation("Start");
                    UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
                    this.$current = new WaitForSeconds(2.5f);
                    this.$PC = 3;
                    goto Label_0188;

                case 3:
                    UI.Sound.Play(SoundEffectType.RewardCharacterFlip);
                    this.$current = new WaitForSeconds(3f);
                    this.$PC = 4;
                    goto Label_0188;

                case 4:
                    UI.Busy = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0188:
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

