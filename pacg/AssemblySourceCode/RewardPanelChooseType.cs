using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RewardPanelChooseType : GuiPanel
{
    private void OnCardFilterAlliesButtonPushed()
    {
        this.SelectCardType(CardType.Ally, 5);
    }

    private void OnCardFilterArmorsButtonPushed()
    {
        this.SelectCardType(CardType.Armor, 3);
    }

    private void OnCardFilterBlessingsButtonPushed()
    {
        this.SelectCardType(CardType.Blessing, 6);
    }

    private void OnCardFilterItemsButtonPushed()
    {
        this.SelectCardType(CardType.Item, 4);
    }

    private void OnCardFilterSpellsButtonPushed()
    {
        this.SelectCardType(CardType.Spell, 2);
    }

    private void OnCardFilterWeaponsButtonPushed()
    {
        this.SelectCardType(CardType.Weapon, 1);
    }

    private void SelectCardType(CardType type, int animNumber)
    {
        if (!base.Locked)
        {
            base.Locked = true;
            this.Show(false);
            Game.Instance.StartCoroutine(this.SelectCardType_Coroutine(type, animNumber));
        }
    }

    [DebuggerHidden]
    private IEnumerator SelectCardType_Coroutine(CardType type, int animNumber) => 
        new <SelectCardType_Coroutine>c__IteratorA0 { 
            animNumber = animNumber,
            type = type,
            <$>animNumber = animNumber,
            <$>type = type,
            <>f__this = this
        };

    [CompilerGenerated]
    private sealed class <SelectCardType_Coroutine>c__IteratorA0 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>animNumber;
        internal CardType <$>type;
        internal RewardPanelChooseType <>f__this;
        internal Animator <anim>__0;
        internal RewardCardChooseType <reward>__2;
        internal GuiWindowReward <window>__1;
        internal int animNumber;
        internal CardType type;

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
                    this.<anim>__0 = this.<>f__this.GetComponentInParent<Animator>();
                    if (this.<anim>__0 != null)
                    {
                        this.<anim>__0.SetInteger("Selected_Type", this.animNumber);
                        this.<anim>__0.SetTrigger("Selected");
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.9f));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.<>f__this.Locked = false;
                    this.<window>__1 = UI.Window as GuiWindowReward;
                    if (this.<window>__1 != null)
                    {
                        this.<reward>__2 = this.<window>__1.Reward as RewardCardChooseType;
                        if (this.<reward>__2 != null)
                        {
                            this.<reward>__2.SetCardType(this.type);
                        }
                    }
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

