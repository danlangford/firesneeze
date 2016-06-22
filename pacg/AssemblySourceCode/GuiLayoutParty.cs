using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutParty : GuiLayout
{
    [Tooltip("party member number that this layout corresponds to")]
    public int Number;

    [DebuggerHidden]
    private IEnumerator GiveCard(Card card) => 
        new <GiveCard>c__Iterator4C { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    private bool IsGuiDropValid(Card card)
    {
        if (card == null)
        {
            return false;
        }
        if (Turn.State != GameStateType.Give)
        {
            return false;
        }
        if (Turn.Number == this.Number)
        {
            return false;
        }
        if (this.Number >= Party.Characters.Count)
        {
            return false;
        }
        if (((Turn.TargetType == TargetType.AnotherAtLocation) || (Turn.TargetType == TargetType.AllAtLocation)) && (Party.Characters[this.Number].Location != Turn.Character.Location))
        {
            return false;
        }
        TurnStateData stateData = Turn.GetStateData();
        if (stateData != null)
        {
            if (stateData.Owner != Turn.Character.ID)
            {
                return false;
            }
        }
        else if (!Rules.IsTurnOwner())
        {
            return false;
        }
        return true;
    }

    public override bool OnGuiDrag(Card card) => 
        false;

    public override bool OnGuiDrop(Card card)
    {
        if (!this.IsGuiDropValid(card))
        {
            return false;
        }
        base.StartCoroutine(this.GiveCard(card));
        this.OnGuiHover(false);
        return true;
    }

    public override bool OnGuiHover(Card card)
    {
        if (this.IsGuiDropValid(card))
        {
            base.GlowAnimator.SetActive(true);
        }
        return true;
    }

    public override bool OnGuiHover(bool hover)
    {
        base.GlowAnimator.SetActive(hover && (Turn.State == GameStateType.Give));
        return true;
    }

    protected override void Start()
    {
        base.Start();
        if (this.Number >= Party.Characters.Count)
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    [CompilerGenerated]
    private sealed class <GiveCard>c__Iterator4C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal GuiLayoutParty <>f__this;
        internal float <delay>__0;
        internal Card card;

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
                    this.card.Deck.Remove(this.card);
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_0163;

                case 1:
                    LeanTween.cancel(this.card.gameObject);
                    this.card.Show(true);
                    this.card.SortingOrder = Constants.SPRITE_SORTING_DRAG;
                    this.card.MoveCard(this.<>f__this.transform.position, 0.1f);
                    LeanTween.scale(this.card.gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                    this.$current = new WaitForSeconds(0.15f);
                    this.$PC = 2;
                    goto Label_0163;

                case 2:
                    Party.Characters[this.<>f__this.Number].Hand.Add(this.card);
                    this.<delay>__0 = (UI.Window as GuiWindowLocation).targetPanel.Bump(this.<>f__this.Number);
                    this.$current = new WaitForSeconds(this.<delay>__0 + 0.15f);
                    this.$PC = 3;
                    goto Label_0163;

                case 3:
                    this.card.Show(false);
                    Turn.Proceed();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0163:
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

