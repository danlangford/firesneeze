using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameStateFlee : GameState
{
    private List<Card> cardList;
    private int cardSortingOrder = Constants.SPRITE_SORTING_DRAG;
    private static readonly Vector3 shuffleSize = new Vector3(0.4f, 0.4f, 1f);

    private Card CreateCard(Card card)
    {
        if (card != null)
        {
            this.cardList.Add(card);
            card.transform.parent = null;
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero;
            card.SortingOrder = this.cardSortingOrder++;
            card.Animations(false);
            card.Show(false);
        }
        return card;
    }

    private Card CreateCard(string id)
    {
        Card card = CardTable.Create(id);
        return this.CreateCard(card);
    }

    public override void Enter()
    {
        base.Enter();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            this.cardList = new List<Card>(Scenario.Current.Locations.Length);
            Game.Instance.StartCoroutine(this.VisualSequence(window));
        }
        else
        {
            this.Proceed();
        }
    }

    public override bool IsResolveSuccess() => 
        (Turn.LastCombatResult == CombatResultType.Win);

    public override void Proceed()
    {
        if (this.cardList != null)
        {
            for (int i = 0; i < this.cardList.Count; i++)
            {
                this.cardList[i].Destroy();
            }
            this.cardList.Clear();
            this.cardList = null;
        }
        Scenario.Current.ResetTemporaryLocationClosures();
        if ((Turn.LastCombatResult == CombatResultType.Win) && Rules.IsEncounterInCurrentLocation())
        {
            Turn.PushStateDestination(GameStateType.Dispose);
            Turn.State = GameStateType.Closing;
        }
        else
        {
            Turn.State = GameStateType.Dispose;
            Game.Events.Next();
        }
    }

    [DebuggerHidden]
    private IEnumerator VisualSequence(GuiWindowLocation window) => 
        new <VisualSequence>c__Iterator37 { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator VisualSequenceBlessings(GuiWindowLocation window) => 
        new <VisualSequenceBlessings>c__Iterator39 { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator VisualSequenceFlee(GuiWindowLocation window) => 
        new <VisualSequenceFlee>c__Iterator3A { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator VisualSequenceVillain(GuiWindowLocation window) => 
        new <VisualSequenceVillain>c__Iterator38 { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.Flee;

    [CompilerGenerated]
    private sealed class <VisualSequence>c__Iterator37 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal GameStateFlee <>f__this;
        internal GuiWindowLocation window;

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
                    this.window.ProcessLayoutDecks();
                    this.window.dicePanel.Clear();
                    this.window.Pause(true);
                    this.window.ShowMap(true);
                    this.window.mapPanel.CenterAllIcons();
                    this.window.mapPanel.RefreshIconLines();
                    this.window.mapPanel.Pause(true);
                    this.window.ShowCancelButton(false);
                    this.window.ShowProceedButton(false);
                    Turn.Character.Hand.Layout.Show(false);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.VisualSequenceVillain(this.window));
                    this.$PC = 1;
                    goto Label_0182;

                case 1:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.VisualSequenceBlessings(this.window));
                    this.$PC = 2;
                    goto Label_0182;

                case 2:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.VisualSequenceFlee(this.window));
                    this.$PC = 3;
                    goto Label_0182;

                case 3:
                    Turn.Character.Hand.Layout.Show(true);
                    this.window.ShowMap(false);
                    this.window.Pause(false);
                    this.<>f__this.Proceed();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0182:
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

    [CompilerGenerated]
    private sealed class <VisualSequenceBlessings>c__Iterator39 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal GameStateFlee <>f__this;
        internal Card <blessing>__6;
        internal int <i>__1;
        internal int <i>__5;
        internal string <locID>__2;
        internal int <numBlessings>__0;
        internal Vector3 <spawnLocation>__3;
        internal int <startingCount>__4;
        internal GuiWindowLocation window;

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
                    this.<numBlessings>__0 = -1;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < Scenario.Current.Locations.Length)
                    {
                        if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[this.<i>__1].LocationName))
                        {
                            this.<locID>__2 = Scenario.Current.Locations[this.<i>__1].LocationName;
                            if (!Scenario.Current.IsLocationClosed(this.<locID>__2))
                            {
                                this.<numBlessings>__0++;
                            }
                        }
                        this.<i>__1++;
                    }
                    if (this.<>f__this.IsResolveSuccess() && Rules.IsEncounterInCurrentLocation())
                    {
                        this.<numBlessings>__0--;
                    }
                    this.<spawnLocation>__3 = this.window.layoutBlessings.transform.position;
                    if (this.<>f__this.IsResolveSuccess())
                    {
                        this.<spawnLocation>__3 = this.window.boxPanel.transform.position;
                    }
                    if (this.<>f__this.IsResolveSuccess())
                    {
                        this.window.boxPanel.Show(true);
                        this.window.boxPanel.PlayAnimation("AddCard", true);
                        this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1f));
                        this.$PC = 1;
                        goto Label_034B;
                    }
                    break;

                case 1:
                    break;

                case 2:
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.1f));
                    this.$PC = 3;
                    goto Label_034B;

                case 3:
                    goto Label_02D6;

                case 4:
                    this.$PC = -1;
                    goto Label_0349;

                default:
                    goto Label_0349;
            }
            this.<startingCount>__4 = Scenario.Current.Blessings.Count + this.<numBlessings>__0;
            this.<i>__5 = 0;
            while (this.<i>__5 < this.<numBlessings>__0)
            {
                this.<blessing>__6 = this.<>f__this.CreateCard(Scenario.Current.Villain);
                this.<blessing>__6.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
                this.<blessing>__6.transform.position = this.<spawnLocation>__3;
                this.<blessing>__6.Show(CardSideType.Back);
                if (!this.<>f__this.IsResolveSuccess())
                {
                    this.window.blessingsPanel.Decrement((this.<startingCount>__4 - this.<i>__5) - 1);
                }
                this.<blessing>__6.MoveCard(Vector3.zero, 0.4f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.scale(this.<blessing>__6.gameObject, GameStateFlee.shuffleSize, 0.4f).setEase(LeanTweenType.easeOutQuad);
                this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.4f));
                this.$PC = 2;
                goto Label_034B;
            Label_02D6:
                this.<i>__5++;
            }
            if (this.<>f__this.IsResolveSuccess())
            {
                this.window.boxPanel.PlayAnimation("AddCard", false);
            }
            this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.2f));
            this.$PC = 4;
            goto Label_034B;
        Label_0349:
            return false;
        Label_034B:
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

    [CompilerGenerated]
    private sealed class <VisualSequenceFlee>c__Iterator3A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal GameStateFlee <>f__this;
        internal Vector3[] <curve>__8;
        internal GameObject <flash>__10;
        internal int <i>__0;
        internal int <i>__12;
        internal int <i>__13;
        internal int <i>__4;
        internal int <i>__7;
        internal int <i>__9;
        internal List<ScenarioMapIcon> <icons>__3;
        internal ScenarioMapIcon[] <locations>__2;
        internal string <locID>__5;
        internal Vector3 <mapSize>__6;
        internal float <shuffleTime>__1;
        internal Vector3 <upSize>__11;
        internal GuiWindowLocation window;

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
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.cardList.Count)
                    {
                        this.<>f__this.cardList[this.<i>__0].Show(false);
                        this.<i>__0++;
                    }
                    this.window.shufflePanel.transform.position = Vector3.zero;
                    this.window.shufflePanel.transform.localScale = GameStateFlee.shuffleSize;
                    this.window.shufflePanel.Show(true);
                    this.window.shufflePanel.SortingOrder = 100;
                    this.<shuffleTime>__1 = this.window.shufflePanel.Shuffle(this.<>f__this.cardList.Count);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(this.<shuffleTime>__1));
                    this.$PC = 1;
                    goto Label_0643;

                case 1:
                    this.window.shufflePanel.Show(false);
                    this.<locations>__2 = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
                    this.<icons>__3 = new List<ScenarioMapIcon>(this.<locations>__2.Length);
                    this.<i>__4 = 0;
                    while (this.<i>__4 < this.<locations>__2.Length)
                    {
                        if (Scenario.Current.IsLocationValid(this.<locations>__2[this.<i>__4].ID))
                        {
                            this.<locID>__5 = this.<locations>__2[this.<i>__4].ID;
                            if (this.<>f__this.IsResolveSuccess() && Rules.IsEncounterInCurrentLocation())
                            {
                                if (!Scenario.Current.IsLocationClosed(this.<locID>__5) && (this.<locID>__5 != Location.Current.ID))
                                {
                                    this.<icons>__3.Add(this.<locations>__2[this.<i>__4]);
                                }
                            }
                            else if (!Scenario.Current.IsLocationClosed(this.<locID>__5))
                            {
                                this.<icons>__3.Add(this.<locations>__2[this.<i>__4]);
                            }
                        }
                        this.<i>__4++;
                    }
                    this.<mapSize>__6 = new Vector3(0.15f, 0.15f, 1f);
                    this.<i>__7 = 0;
                    while (this.<i>__7 < this.<>f__this.cardList.Count)
                    {
                        if (this.<i>__7 < this.<icons>__3.Count)
                        {
                            this.<>f__this.cardList[this.<i>__7].Show(true);
                            this.<curve>__8 = Geometry.GetCurve(this.<>f__this.cardList[this.<i>__7].gameObject.transform.position, this.<icons>__3[this.<i>__7].transform.position, 0f);
                            this.<>f__this.cardList[this.<i>__7].MoveCard(this.<curve>__8, 0.4f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                            LeanTween.scale(this.<>f__this.cardList[this.<i>__7].gameObject, this.<mapSize>__6, 0.4f).setEase(LeanTweenType.easeOutQuad);
                        }
                        this.<i>__7++;
                    }
                    UI.Sound.Play(SoundEffectType.GenericCardMoved);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.4f));
                    this.$PC = 2;
                    goto Label_0643;

                case 2:
                    this.<i>__9 = 0;
                    while (this.<i>__9 < this.<>f__this.cardList.Count)
                    {
                        if (this.<i>__9 < this.<icons>__3.Count)
                        {
                            this.<flash>__10 = this.<icons>__3[this.<i>__9].Flash();
                            if (this.<flash>__10 != null)
                            {
                                UnityEngine.Object.Destroy(this.<flash>__10, 1f);
                            }
                        }
                        this.<i>__9++;
                    }
                    UI.Sound.Play(SoundEffectType.CardDroppedIntoLocationMap);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.25f));
                    this.$PC = 3;
                    goto Label_0643;

                case 3:
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.1f));
                    this.$PC = 4;
                    goto Label_0643;

                case 4:
                    this.<upSize>__11 = new Vector3(1.25f * this.<>f__this.cardList[0].transform.localScale.x, 1.25f * this.<>f__this.cardList[0].transform.localScale.y, 1f);
                    this.<i>__12 = 0;
                    while (this.<i>__12 < this.<>f__this.cardList.Count)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<i>__12].gameObject, this.<upSize>__11, 0.3f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__12++;
                    }
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.3f));
                    this.$PC = 5;
                    goto Label_0643;

                case 5:
                    this.<i>__13 = 0;
                    while (this.<i>__13 < this.<>f__this.cardList.Count)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<i>__13].gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__13++;
                    }
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.25f));
                    this.$PC = 6;
                    goto Label_0643;

                case 6:
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.3f));
                    this.$PC = 7;
                    goto Label_0643;

                case 7:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0643:
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

    [CompilerGenerated]
    private sealed class <VisualSequenceVillain>c__Iterator38 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal GameStateFlee <>f__this;
        internal Card <villain>__0;
        internal GuiWindowLocation window;

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
                    this.window.layoutLocation.GlowText(false);
                    if (Turn.Card.Type != CardType.Villain)
                    {
                        this.<villain>__0 = this.<>f__this.CreateCard(Scenario.Current.Villain);
                        break;
                    }
                    if (Turn.Card.Disposition != DispositionType.Destroy)
                    {
                        this.<villain>__0 = this.<>f__this.CreateCard(Turn.Card.ID);
                        break;
                    }
                    this.<villain>__0 = this.<>f__this.CreateCard(Turn.Card);
                    if (this.<villain>__0.Deck != null)
                    {
                        this.<villain>__0.Deck.Remove(this.<villain>__0);
                    }
                    break;

                case 1:
                    this.<villain>__0.Animate(AnimationType.Escape, true);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(1.2f));
                    this.$PC = 2;
                    goto Label_0253;

                case 2:
                    this.<villain>__0.Show(CardSideType.Back);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.2f));
                    this.$PC = 3;
                    goto Label_0253;

                case 3:
                    this.<villain>__0.MoveCard(Vector3.zero, 0.4f).setEase(LeanTweenType.easeOutQuad);
                    LeanTween.scale(this.<villain>__0.gameObject, GameStateFlee.shuffleSize, 0.4f).setEase(LeanTweenType.easeOutQuad);
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.4f));
                    this.$PC = 4;
                    goto Label_0253;

                case 4:
                    this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.2f));
                    this.$PC = 5;
                    goto Label_0253;

                case 5:
                    this.$PC = -1;
                    goto Label_0251;

                default:
                    goto Label_0251;
            }
            this.<villain>__0.transform.localScale = this.window.layoutLocation.Scale;
            this.<villain>__0.transform.position = this.window.layoutLocation.transform.position;
            this.<villain>__0.Show(CardSideType.Front);
            this.$current = Game.Instance.StartCoroutine(GameState.WaitForTime(0.2f));
            this.$PC = 1;
            goto Label_0253;
        Label_0251:
            return false;
        Label_0253:
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

