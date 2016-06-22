using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventVillainIntroducedTutorial : Event
{
    private List<Card> cardList;
    private static int cardSortingOrder = Constants.SPRITE_SORTING_DRAG;
    private Animator locationHilite;
    private static readonly Vector3 shuffleSize = new Vector3(0.4f, 0.4f, 1f);
    private Animator tutorialAnimator;

    private void Cleanup()
    {
        if (this.locationHilite != null)
        {
            UnityEngine.Object.Destroy(this.locationHilite.gameObject);
        }
        this.locationHilite = null;
        if (this.tutorialAnimator != null)
        {
            UnityEngine.Object.Destroy(this.tutorialAnimator.gameObject);
        }
        this.tutorialAnimator = null;
        for (int i = 0; i < this.cardList.Count; i++)
        {
            UnityEngine.Object.Destroy(this.cardList[i].gameObject);
        }
        this.cardList.Clear();
        this.cardList = null;
        Turn.Card.Show(true);
    }

    private Card CreateCard(string id)
    {
        Card item = CardTable.Create(id);
        if (item != null)
        {
            this.cardList.Add(item);
            item.transform.parent = null;
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            item.SortingOrder = cardSortingOrder++;
            item.Animations(false);
            item.Show(false);
        }
        return item;
    }

    [DebuggerHidden]
    private IEnumerator DistributeBlessings(bool isResolveSuccess, int offset) => 
        new <DistributeBlessings>c__Iterator2E { 
            offset = offset,
            isResolveSuccess = isResolveSuccess,
            <$>offset = offset,
            <$>isResolveSuccess = isResolveSuccess,
            <>f__this = this
        };

    public override void OnVillainIntroduced(Card card)
    {
        if ((Turn.CheckBoard.Get<int>("TutorialVillainEncounterNumber") == 0) && (Scenario.Current.NumVillainEncounters == 1))
        {
            Turn.CheckBoard.Set<int>("TutorialVillainEncounterNumber", 1);
            Game.Instance.StartCoroutine(this.VisualSequenceTutorial());
        }
    }

    private void Setup()
    {
        Turn.Card.Show(false);
        this.cardList = new List<Card>(10);
        cardSortingOrder = Constants.SPRITE_SORTING_DRAG;
    }

    [DebuggerHidden]
    private IEnumerator SpawnBlessings(int numBlessings, Vector3 spawnLocation) => 
        new <SpawnBlessings>c__Iterator2D { 
            numBlessings = numBlessings,
            spawnLocation = spawnLocation,
            <$>numBlessings = numBlessings,
            <$>spawnLocation = spawnLocation,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator VisualSequenceTutorial() => 
        new <VisualSequenceTutorial>c__Iterator2A { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator VisualSequenceTutorialPartOne(GuiWindowLocation window) => 
        new <VisualSequenceTutorialPartOne>c__Iterator2B { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    [DebuggerHidden]
    private IEnumerator VisualSequenceTutorialPartTwo(GuiWindowLocation window) => 
        new <VisualSequenceTutorialPartTwo>c__Iterator2C { 
            window = window,
            <$>window = window,
            <>f__this = this
        };

    public override EventType Type =>
        EventType.OnVillainIntroduced;

    [CompilerGenerated]
    private sealed class <DistributeBlessings>c__Iterator2E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal bool <$>isResolveSuccess;
        internal int <$>offset;
        internal EventVillainIntroducedTutorial <>f__this;
        internal Vector3[] <curve>__9;
        internal GameObject <flash>__11;
        internal int <i>__1;
        internal int <i>__10;
        internal int <i>__13;
        internal int <i>__14;
        internal int <i>__5;
        internal int <i>__8;
        internal List<ScenarioMapIcon> <icons>__4;
        internal ScenarioMapIcon[] <locations>__3;
        internal string <locID>__6;
        internal Vector3 <mapSize>__7;
        internal float <shuffleTime>__2;
        internal Vector3 <upSize>__12;
        internal GuiWindowLocation <window>__0;
        internal bool isResolveSuccess;
        internal int offset;

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
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.cardList.Count)
                    {
                        this.<>f__this.cardList[this.<i>__1].Show(false);
                        this.<i>__1++;
                    }
                    this.<window>__0.shufflePanel.transform.position = Vector3.zero;
                    this.<window>__0.shufflePanel.transform.localScale = EventVillainIntroducedTutorial.shuffleSize;
                    this.<window>__0.shufflePanel.Show(true);
                    this.<window>__0.shufflePanel.SortingOrder = 100;
                    this.<shuffleTime>__2 = this.<window>__0.shufflePanel.Shuffle(this.<>f__this.cardList.Count - this.offset);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.<shuffleTime>__2));
                    this.$PC = 1;
                    goto Label_0694;

                case 1:
                    this.<window>__0.shufflePanel.Show(false);
                    this.<locations>__3 = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
                    this.<icons>__4 = new List<ScenarioMapIcon>(this.<locations>__3.Length);
                    this.<i>__5 = 0;
                    while (this.<i>__5 < this.<locations>__3.Length)
                    {
                        if (Scenario.Current.IsLocationValid(this.<locations>__3[this.<i>__5].ID))
                        {
                            this.<locID>__6 = this.<locations>__3[this.<i>__5].ID;
                            if (this.isResolveSuccess)
                            {
                                if (!Scenario.Current.IsLocationClosed(this.<locID>__6) && (this.<locID>__6 != Location.Current.ID))
                                {
                                    this.<icons>__4.Add(this.<locations>__3[this.<i>__5]);
                                }
                            }
                            else if (!Scenario.Current.IsLocationClosed(this.<locID>__6))
                            {
                                this.<icons>__4.Add(this.<locations>__3[this.<i>__5]);
                            }
                        }
                        this.<i>__5++;
                    }
                    this.<mapSize>__7 = new Vector3(0.15f, 0.15f, 1f);
                    this.<i>__8 = this.offset;
                    while (this.<i>__8 < this.<>f__this.cardList.Count)
                    {
                        if ((this.<i>__8 - this.offset) < this.<icons>__4.Count)
                        {
                            this.<>f__this.cardList[this.<i>__8].Show(true);
                            this.<curve>__9 = Geometry.GetCurve(this.<>f__this.cardList[this.<i>__8].gameObject.transform.position, this.<icons>__4[this.<i>__8 - this.offset].transform.position, 0f);
                            this.<>f__this.cardList[this.<i>__8].MoveCard(this.<curve>__9, 0.4f).setEase(LeanTweenType.easeOutQuad);
                            LeanTween.scale(this.<>f__this.cardList[this.<i>__8].gameObject, this.<mapSize>__7, 0.4f).setEase(LeanTweenType.easeOutQuad);
                        }
                        this.<i>__8++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.4f));
                    this.$PC = 2;
                    goto Label_0694;

                case 2:
                    this.<i>__10 = this.offset;
                    while (this.<i>__10 < this.<>f__this.cardList.Count)
                    {
                        if (this.<i>__10 < this.<icons>__4.Count)
                        {
                            this.<flash>__11 = this.<icons>__4[this.<i>__10].Flash();
                            if (this.<flash>__11 != null)
                            {
                                UnityEngine.Object.Destroy(this.<flash>__11, 1f);
                            }
                        }
                        this.<i>__10++;
                    }
                    UI.Sound.Play(SoundEffectType.CardDroppedIntoLocationMap);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
                    this.$PC = 3;
                    goto Label_0694;

                case 3:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                    this.$PC = 4;
                    goto Label_0694;

                case 4:
                    this.<upSize>__12 = new Vector3(1.25f * this.<>f__this.cardList[this.offset].transform.localScale.x, 1.25f * this.<>f__this.cardList[this.offset].transform.localScale.y, 1f);
                    this.<i>__13 = this.offset;
                    while (this.<i>__13 < this.<>f__this.cardList.Count)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<i>__13].gameObject, this.<upSize>__12, 0.3f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__13++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 5;
                    goto Label_0694;

                case 5:
                    this.<i>__14 = this.offset;
                    while (this.<i>__14 < this.<>f__this.cardList.Count)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<i>__14].gameObject, Vector3.zero, 0.25f).setEase(LeanTweenType.easeOutQuad);
                        this.<i>__14++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
                    this.$PC = 6;
                    goto Label_0694;

                case 6:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 7;
                    goto Label_0694;

                case 7:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0694:
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
    private sealed class <SpawnBlessings>c__Iterator2D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal int <$>numBlessings;
        internal Vector3 <$>spawnLocation;
        internal EventVillainIntroducedTutorial <>f__this;
        internal Card <blessing>__1;
        internal int <i>__0;
        internal int numBlessings;
        internal Vector3 spawnLocation;

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
                    break;

                case 1:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                    this.$PC = 2;
                    goto Label_014F;

                case 2:
                    this.<i>__0++;
                    break;

                default:
                    goto Label_014D;
            }
            if (this.<i>__0 < this.numBlessings)
            {
                this.<blessing>__1 = this.<>f__this.CreateCard(Scenario.Current.Villain);
                this.<blessing>__1.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
                this.<blessing>__1.transform.position = this.spawnLocation;
                this.<blessing>__1.Show(CardSideType.Back);
                this.<blessing>__1.MoveCard(Vector3.zero, 0.4f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.scale(this.<blessing>__1.gameObject, EventVillainIntroducedTutorial.shuffleSize, 0.4f).setEase(LeanTweenType.easeOutQuad);
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.4f));
                this.$PC = 1;
                goto Label_014F;
            }
            this.$PC = -1;
        Label_014D:
            return false;
        Label_014F:
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
    private sealed class <VisualSequenceTutorial>c__Iterator2A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal EventVillainIntroducedTutorial <>f__this;
        internal GuiWindowLocation <window>__0;

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
                    this.<window>__0 = UI.Window as GuiWindowLocation;
                    Turn.State = GameStateType.Null;
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    goto Label_012D;

                case 1:
                    this.<window>__0.ShowMap(true);
                    this.<window>__0.mapPanel.Pause(true);
                    this.<window>__0.Pause(true);
                    this.<>f__this.Setup();
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.VisualSequenceTutorialPartOne(this.<window>__0));
                    this.$PC = 2;
                    goto Label_012D;

                case 2:
                    this.<>f__this.Cleanup();
                    this.<>f__this.Setup();
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.VisualSequenceTutorialPartTwo(this.<window>__0));
                    this.$PC = 3;
                    goto Label_012D;

                case 3:
                    this.<>f__this.Cleanup();
                    this.<window>__0.ShowMap(false);
                    this.<window>__0.Pause(false);
                    Turn.State = GameStateType.Villain;
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_012D:
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
    private sealed class <VisualSequenceTutorialPartOne>c__Iterator2B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal EventVillainIntroducedTutorial <>f__this;
        internal Transform <arrow>__10;
        internal Transform <arrow>__7;
        internal GameObject <go>__2;
        internal GameObject <go>__4;
        internal int <i>__1;
        internal int <i>__6;
        internal int <i>__9;
        internal int <nArrow>__5;
        internal int <nArrow>__8;
        internal GameObject <tutorialAnimatorPrefab>__3;
        internal GameObject <vfxPrefab>__0;
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
                    this.<vfxPrefab>__0 = Resources.Load<GameObject>("Art/VFX/vfx_Highlight location");
                    if (this.<vfxPrefab>__0 != null)
                    {
                        this.<i>__1 = 0;
                        while (this.<i>__1 < this.window.mapPanel.Icons.Count)
                        {
                            if (this.window.mapPanel.Icons[this.<i>__1].ID == Turn.Owner.Location)
                            {
                                this.<go>__2 = Game.Instance.Create(this.<vfxPrefab>__0);
                                if (this.<go>__2 != null)
                                {
                                    this.<go>__2.transform.position = this.window.mapPanel.Icons[this.<i>__1].transform.position;
                                    this.<>f__this.locationHilite = this.<go>__2.GetComponent<Animator>();
                                }
                                break;
                            }
                            this.<i>__1++;
                        }
                    }
                    break;

                case 1:
                    if (this.<>f__this.locationHilite != null)
                    {
                        this.<>f__this.locationHilite.gameObject.SetActive(false);
                    }
                    this.<tutorialAnimatorPrefab>__3 = Resources.Load<GameObject>("Art/VFX/vfx_Tut_Villain_DefeatOrNot");
                    if (this.<tutorialAnimatorPrefab>__3 != null)
                    {
                        this.<go>__4 = Game.Instance.Create(this.<tutorialAnimatorPrefab>__3);
                        if (this.<go>__4 != null)
                        {
                            this.<>f__this.tutorialAnimator = this.<go>__4.GetComponent<Animator>();
                            this.<>f__this.tutorialAnimator.enabled = true;
                            this.<>f__this.tutorialAnimator.transform.position = this.window.mapPanel.GetMapIcon("LO1B_Farmhouse").transform.position;
                        }
                    }
                    if (this.<>f__this.tutorialAnimator != null)
                    {
                        this.<nArrow>__5 = 1;
                        this.<i>__6 = 0;
                        while (this.<i>__6 < this.window.mapPanel.Icons.Count)
                        {
                            this.<arrow>__7 = this.<>f__this.tutorialAnimator.transform.FindChild("TutArrow" + this.<nArrow>__5);
                            if (this.<arrow>__7 != null)
                            {
                                if (this.window.mapPanel.Icons[this.<i>__6].ID == "LO1B_Farmhouse")
                                {
                                    this.<arrow>__7.gameObject.SetActive(false);
                                }
                                else
                                {
                                    this.<arrow>__7.gameObject.SetActive(true);
                                }
                                this.<arrow>__7.transform.position = this.window.mapPanel.Icons[this.<i>__6].transform.position;
                                this.<nArrow>__5++;
                            }
                            this.<i>__6++;
                        }
                    }
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(70, 0.5f, 0.3f));
                    this.$PC = 2;
                    goto Label_06B0;

                case 2:
                    if (this.<>f__this.tutorialAnimator != null)
                    {
                        this.<>f__this.tutorialAnimator.SetTrigger("Next");
                    }
                    this.window.boxPanel.Show(true);
                    this.window.boxPanel.PlayAnimation("AddCard", true);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 3;
                    goto Label_06B0;

                case 3:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.SpawnBlessings(2, this.window.boxPanel.transform.position));
                    this.$PC = 4;
                    goto Label_06B0;

                case 4:
                    this.window.boxPanel.PlayAnimation("AddCard", false);
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x47, 0.5f, 0.3f));
                    this.$PC = 5;
                    goto Label_06B0;

                case 5:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.DistributeBlessings(true, 0));
                    this.$PC = 6;
                    goto Label_06B0;

                case 6:
                    if (this.<>f__this.locationHilite != null)
                    {
                        this.<>f__this.locationHilite.SetTrigger("Next");
                    }
                    if (this.<>f__this.tutorialAnimator != null)
                    {
                        this.<nArrow>__8 = 1;
                        this.<i>__9 = 0;
                        while (this.<i>__9 < this.window.mapPanel.Icons.Count)
                        {
                            this.<arrow>__10 = this.<>f__this.tutorialAnimator.transform.FindChild("TutArrow" + this.<nArrow>__8);
                            if (this.<arrow>__10 != null)
                            {
                                this.<arrow>__10.gameObject.SetActive(true);
                                this.<arrow>__10.transform.position = this.window.mapPanel.Icons[this.<i>__9].transform.position;
                                this.<nArrow>__8++;
                            }
                            this.<i>__9++;
                        }
                    }
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x48, 0.5f, 0.3f));
                    this.$PC = 7;
                    goto Label_06B0;

                case 7:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.SpawnBlessings(2, this.window.layoutBlessings.transform.position));
                    this.$PC = 8;
                    goto Label_06B0;

                case 8:
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x49, 0.5f, 0.3f));
                    this.$PC = 9;
                    goto Label_06B0;

                case 9:
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.DistributeBlessings(false, 2));
                    this.$PC = 10;
                    goto Label_06B0;

                case 10:
                    this.$PC = -1;
                    goto Label_06AE;

                default:
                    goto Label_06AE;
            }
            this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x45, 0.5f, 0.3f));
            this.$PC = 1;
            goto Label_06B0;
        Label_06AE:
            return false;
        Label_06B0:
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
    private sealed class <VisualSequenceTutorialPartTwo>c__Iterator2C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowLocation <$>window;
        internal EventVillainIntroducedTutorial <>f__this;
        internal GameObject <go>__3;
        internal GameObject <go>__9;
        internal int <i>__10;
        internal int <i>__6;
        internal int <i>__8;
        internal CloseType <originalCloseTypeFarmhouse>__4;
        internal CloseType <originalCloseTypeWarrens>__5;
        internal string <originalLocationAmeiko>__0;
        internal string <originalLocationOrik>__1;
        internal GameObject <tutorialAnimatorPrefab>__2;
        internal GameObject <vfxPrefab>__7;
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
                    this.<originalLocationAmeiko>__0 = Party.Characters["CH1T_Ameiko"].Location;
                    this.<originalLocationOrik>__1 = Party.Characters["CH1T_Orik"].Location;
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x4a, 0.5f, 0.3f));
                    this.$PC = 1;
                    goto Label_0604;

                case 1:
                    this.<tutorialAnimatorPrefab>__2 = Resources.Load<GameObject>("Art/VFX/vfx_Tut_Closing");
                    if (this.<tutorialAnimatorPrefab>__2 != null)
                    {
                        this.<go>__3 = Game.Instance.Create(this.<tutorialAnimatorPrefab>__2);
                        if (this.<go>__3 != null)
                        {
                            this.<>f__this.tutorialAnimator = this.<go>__3.GetComponent<Animator>();
                            this.<>f__this.tutorialAnimator.enabled = true;
                            this.<>f__this.tutorialAnimator.transform.position = this.window.mapPanel.GetMapIcon("LO1B_Farmhouse").transform.position;
                        }
                    }
                    this.$current = new WaitForSeconds(0.15f);
                    this.$PC = 2;
                    goto Label_0604;

                case 2:
                    this.<originalCloseTypeFarmhouse>__4 = Scenario.Current.GetLocationCloseType("LO1B_Farmhouse");
                    this.<originalCloseTypeWarrens>__5 = Scenario.Current.GetLocationCloseType("LO1B_Warrens");
                    Scenario.Current.CloseLocation("LO1B_Farmhouse", CloseType.Permanent);
                    Scenario.Current.CloseLocation("LO1B_Warrens", CloseType.Permanent);
                    this.window.mapPanel.Refresh();
                    this.<i>__6 = 0;
                    while (this.<i>__6 < this.window.mapPanel.Icons.Count)
                    {
                        this.window.mapPanel.Icons[this.<i>__6].Decorations = false;
                        this.<i>__6++;
                    }
                    this.$current = new WaitForSeconds(0.7f);
                    this.$PC = 3;
                    goto Label_0604;

                case 3:
                {
                    Character[] characters = new Character[] { Party.Characters["CH1T_Ameiko"], Party.Characters["CH1T_Orik"] };
                    string[] locations = new string[] { "LO1B_DeeperDungeons", "LO1B_DeeperDungeons" };
                    this.$current = Game.Instance.StartCoroutine(this.window.mapPanel.Animate(characters, locations));
                    this.$PC = 4;
                    goto Label_0604;
                }
                case 4:
                    this.<vfxPrefab>__7 = Resources.Load<GameObject>("Art/VFX/vfx_Highlight location");
                    if (this.<vfxPrefab>__7 != null)
                    {
                        this.<i>__8 = 0;
                        while (this.<i>__8 < this.window.mapPanel.Icons.Count)
                        {
                            if (this.window.mapPanel.Icons[this.<i>__8].ID == "LO1B_DeeperDungeons")
                            {
                                this.<go>__9 = Game.Instance.Create(this.<vfxPrefab>__7);
                                if (this.<go>__9 != null)
                                {
                                    this.<go>__9.transform.position = this.window.mapPanel.Icons[this.<i>__8].transform.position;
                                    this.<>f__this.locationHilite = this.<go>__9.GetComponent<Animator>();
                                }
                                break;
                            }
                            this.<i>__8++;
                        }
                    }
                    break;

                case 5:
                    if (this.<>f__this.tutorialAnimator != null)
                    {
                        this.<>f__this.tutorialAnimator.SetTrigger("Next");
                    }
                    this.$current = new WaitForSeconds(0.15f);
                    this.$PC = 6;
                    goto Label_0604;

                case 6:
                {
                    Scenario.Current.CloseLocation("LO1B_Warrens", CloseType.None);
                    this.window.mapPanel.Refresh();
                    Character[] characterArray2 = new Character[] { Party.Characters["CH1T_Ameiko"], Party.Characters["CH1T_Orik"] };
                    string[] textArray2 = new string[] { "LO1B_Warrens", "LO1B_DeeperDungeons" };
                    this.$current = Game.Instance.StartCoroutine(this.window.mapPanel.Animate(characterArray2, textArray2));
                    this.$PC = 7;
                    goto Label_0604;
                }
                case 7:
                    this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x4c, 0.5f, 0.3f));
                    this.$PC = 8;
                    goto Label_0604;

                case 8:
                {
                    if (this.<>f__this.tutorialAnimator != null)
                    {
                        this.<>f__this.tutorialAnimator.SetTrigger("Next");
                    }
                    Scenario.Current.CloseLocation("LO1B_Farmhouse", this.<originalCloseTypeFarmhouse>__4);
                    Scenario.Current.CloseLocation("LO1B_Warrens", this.<originalCloseTypeWarrens>__5);
                    this.window.mapPanel.Refresh();
                    Character[] characterArray3 = new Character[] { Party.Characters["CH1T_Ameiko"], Party.Characters["CH1T_Orik"] };
                    string[] textArray3 = new string[] { this.<originalLocationAmeiko>__0, this.<originalLocationOrik>__1 };
                    this.$current = Game.Instance.StartCoroutine(this.window.mapPanel.Animate(characterArray3, textArray3));
                    this.$PC = 9;
                    goto Label_0604;
                }
                case 9:
                    this.<i>__10 = 0;
                    while (this.<i>__10 < this.window.mapPanel.Icons.Count)
                    {
                        this.window.mapPanel.Icons[this.<i>__10].Decorations = true;
                        this.<i>__10++;
                    }
                    this.$PC = -1;
                    goto Label_0602;

                default:
                    goto Label_0602;
            }
            this.$current = Game.Instance.StartCoroutine(Tutorial.WaitForOverlay(0x4b, 0.5f, 0.3f));
            this.$PC = 5;
            goto Label_0604;
        Label_0602:
            return false;
        Label_0604:
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

