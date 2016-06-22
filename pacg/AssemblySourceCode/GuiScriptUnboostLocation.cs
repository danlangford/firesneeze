using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiScriptUnboostLocation : GuiScript
{
    private Vector3 BoxPosition = new Vector3(0f, 12f, 0f);
    [Tooltip("the indexes of the cards that need to be removed going from high to low")]
    public int[] CardsToRemove;
    [Tooltip("the effect that should accumulate all the cards removed")]
    public Effect Destination;

    private void OnTrayCloseFinished()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutExamine.Clear();
            window.locationPanel.RefreshCardList();
            Location.Load(Turn.Character.Location);
            if (Scenario.Current.IsScenarioOver())
            {
                Turn.State = GameStateType.End;
            }
            else
            {
                Turn.State = GameStateType.Setup;
                window.Refresh();
            }
        }
    }

    public override bool Play()
    {
        Game.Instance.StartCoroutine(this.PlayCoroutine());
        return true;
    }

    [DebuggerHidden]
    private IEnumerator PlayCoroutine() => 
        new <PlayCoroutine>c__Iterator82 { <>f__this = this };

    private void RefreshCardList()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.locationPanel.RefreshCardList();
        }
    }

    public override void Stop()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            LeanTween.delayedCall(window.layoutExamine.Close(), new Action(this.OnTrayCloseFinished));
        }
    }

    [CompilerGenerated]
    private sealed class <PlayCoroutine>c__Iterator82 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiScriptUnboostLocation <>f__this;
        internal int <cardsOfThisType>__3;
        internal int <i>__0;
        internal int <j>__2;
        internal CardType <type>__1;

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
                    Turn.State = GameStateType.Null;
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                    this.$PC = 1;
                    goto Label_02C0;

                case 1:
                    this.<i>__0 = 0;
                    goto Label_0248;

                case 2:
                    this.<>f__this.Destination.sources.Add(Location.Current.Deck[this.<>f__this.CardsToRemove[this.<i>__0]].ID);
                    this.<type>__1 = Location.Current.Deck[this.<>f__this.CardsToRemove[this.<i>__0]].Type;
                    Campaign.Box.Add(Location.Current.Deck[this.<>f__this.CardsToRemove[this.<i>__0]], true);
                    this.<j>__2 = 1;
                    while (this.<j>__2 < Constants.NUM_CARD_TYPES)
                    {
                        if (this.<j>__2 != this.<type>__1)
                        {
                            this.<cardsOfThisType>__3 = Scenario.Current.GetCardCount(Location.Current.ID, (CardType) this.<j>__2);
                            if (this.<cardsOfThisType>__3 > 0)
                            {
                                Scenario.Current.AddCardCount(Location.Current.ID, (CardType) this.<j>__2, -1);
                                Scenario.Current.AddCardCount(Location.Current.ID, CardType.None, 1);
                            }
                        }
                        this.<j>__2++;
                    }
                    break;

                case 3:
                    Scenario.Current.ApplyEffect(this.<>f__this.Destination);
                    this.<>f__this.Stop();
                    this.$PC = -1;
                    goto Label_02BE;

                default:
                    goto Label_02BE;
            }
        Label_023A:
            this.<i>__0++;
        Label_0248:
            if (this.<i>__0 < this.<>f__this.CardsToRemove.Length)
            {
                if ((this.<>f__this.CardsToRemove[this.<i>__0] >= Location.Current.Deck.Count) || (this.<>f__this.CardsToRemove[this.<i>__0] < 0))
                {
                    goto Label_023A;
                }
                Location.Current.Deck[this.<>f__this.CardsToRemove[this.<i>__0]].MoveCard(this.<>f__this.BoxPosition, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                this.$PC = 2;
            }
            else
            {
                this.<>f__this.RefreshCardList();
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.2f));
                this.$PC = 3;
            }
            goto Label_02C0;
        Label_02BE:
            return false;
        Label_02C0:
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

