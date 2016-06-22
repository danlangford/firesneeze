using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelScenarioPreview : GuiPanel
{
    private int arrayNumber;
    [Tooltip("sound when villain appears")]
    public AudioClip BossFocusSound;
    [Tooltip("reference to the card animator in our hierarchy")]
    public Animator CardAnimator;
    [Tooltip("random sound played when henchmen cards appear")]
    public AudioClip[] CardFocusSounds;
    [Tooltip("reference to the card holder (under the animator) in our hierarchy")]
    public Transform CardHolder;
    private List<Card> cardList = new List<Card>(10);
    private int cardSortingOrder = 10;
    [Tooltip("reference to the continue button animator in this scene")]
    public Animator ContinueButtonAnimator;
    [Tooltip("reference to the scenario description text label in our hierarchy")]
    public GuiLabel DescriptionLabel;
    [Tooltip("reference to the large text box art in this scene")]
    public GameObject DescriptionLargeBoxArt;
    [Tooltip("reference to the small text box art in this scene")]
    public GameObject DescriptionSmallBoxArt;
    private bool doneHenchmen;
    private static readonly string fakeCard = "HE1B_Bandit";
    [Tooltip("the henchmen cards are positioned at these transforms")]
    public GameObject[] HenchmenPositions;
    public GameObject HenchmenStowPosition;
    private int screenNumber;
    [Tooltip("reference to the card shuffler in this scene")]
    public GuiPanelShuffler ShuffleAnimator;
    private bool skip;
    [Tooltip("reference to the skip button in this scene")]
    public GuiButton SkipButton;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the text prefab in our hierarchy")]
    public Animator TextAnimator;
    [Tooltip("the villain cards are positioned at these transforms")]
    public GameObject[] VillainPositions;
    public GameObject VillainStowPosition;
    [Tooltip("reference to the tiny sub-panels for revealing extra powers")]
    public GuiPanelScenarioWildcard[] WildcardBoxes;
    private Card zoomedCard;

    private int AddHenchmanToList(int henchmenCount)
    {
        ScenarioPreviewCustom component = Scenario.Current.GetComponent<ScenarioPreviewCustom>();
        if (component != null)
        {
            return component.AddHenchmanToList(henchmenCount, new Func<string, int, Card>(this.CreateCard), this.cardList);
        }
        int num = 0;
        string str = null;
        for (int i = henchmenCount; i < (Scenario.Current.LocationCards.Count - Scenario.Current.Villains.Length); i++)
        {
            int index = Mathf.Min(i, Scenario.Current.Henchmen.Length - 1);
            string id = Scenario.Current.Henchmen[index];
            if ((str != null) && (id != str))
            {
                return num;
            }
            Card item = this.CreateCard(id, num++);
            this.cardList.Add(item);
            str = id;
        }
        return num;
    }

    private void Close()
    {
        this.Show(false);
        UI.Busy = false;
        for (int i = 0; i < this.cardList.Count; i++)
        {
            UnityEngine.Object.Destroy(this.cardList[i].gameObject);
        }
        this.cardList.Clear();
        GuiWindowScenario window = UI.Window as GuiWindowScenario;
        if (window != null)
        {
            window.OnFinishButtonPushed();
        }
    }

    private Card CreateCard(string id, int index)
    {
        Card card = CardTable.Create(id);
        if (card != null)
        {
            card.transform.parent = this.CardHolder;
            card.transform.localScale = Vector3.one;
            card.transform.localPosition = Vector3.zero + new Vector3(-0.25f * index, -0.25f * index, 0f);
            card.SortingOrder = this.cardSortingOrder++;
            card.Animations(false);
            card.Show(true);
        }
        return card;
    }

    private AudioClip GetCardFocusSound(CardType type)
    {
        if (type == CardType.Villain)
        {
            return this.BossFocusSound;
        }
        return this.CardFocusSounds[UnityEngine.Random.Range(0, this.CardFocusSounds.Length)];
    }

    private string GetDescriptionText()
    {
        for (int i = 0; i < Scenario.Current.StartingPowers.Length; i++)
        {
            ScenarioPowerValueType type = Scenario.Current.StartingPowers[i];
            if ((type.Active && (type.Difficulty == Scenario.Current.Difficulty)) && !string.IsNullOrEmpty(type.Description))
            {
                return type.Description;
            }
        }
        return Scenario.Current.DuringText;
    }

    private string[] GetRandomPowerText()
    {
        List<string> list = new List<string>();
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            if (Scenario.Current.Powers[i].Wildcard)
            {
                list.Add(Scenario.Current.Powers[i].ID);
            }
        }
        if (Scenario.Current.Linear)
        {
            list.Add("PS1B_LinearMapMovement");
        }
        return list.ToArray();
    }

    private Card GetTopCard(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
        if (hitd.collider != null)
        {
            return hitd.collider.transform.parent.parent.GetComponent<Card>();
        }
        return null;
    }

    [DebuggerHidden]
    private IEnumerator HideContinueButtons() => 
        new <HideContinueButtons>c__Iterator69 { <>f__this = this };

    public override void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        for (int i = 0; i < this.WildcardBoxes.Length; i++)
        {
            this.WildcardBoxes[i].Initialize();
        }
        this.SkipButton.Show(false);
        this.screenNumber = 0;
        this.skip = false;
    }

    private bool IsVillainHere() => 
        !string.IsNullOrEmpty(Scenario.Current.Villain);

    [DebuggerHidden]
    private IEnumerator LocationHilightLoop(GameObject arrow) => 
        new <LocationHilightLoop>c__Iterator6D { 
            arrow = arrow,
            <$>arrow = arrow
        };

    private int MaxHenchmen()
    {
        ScenarioPreviewCustom component = Scenario.Current.GetComponent<ScenarioPreviewCustom>();
        if (component != null)
        {
            return component.MaxHenchmen();
        }
        if (this.IsVillainHere())
        {
            return (Scenario.Current.LocationCards.Count - Scenario.Current.Villains.Length);
        }
        return Scenario.Current.LocationCards.Count;
    }

    private void OnContinueButtonPushed()
    {
        if (!UI.Window.Paused)
        {
            if (UI.Zoomed)
            {
                this.UnZoomCard();
            }
            Tutorial.Hide();
            if (!UI.Busy && !this.skip)
            {
                this.ScreenController();
            }
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.zoomedCard != null)
        {
            if (!LeanTween.isTweening(this.zoomedCard.gameObject))
            {
                this.UnZoomCard();
            }
        }
        else
        {
            Card topCard = this.GetTopCard(touchPos);
            if ((topCard != null) && !LeanTween.isTweening(topCard.gameObject))
            {
                this.ZoomCard(topCard);
            }
        }
    }

    private void OnSkipButtonPushed()
    {
        if (!UI.Window.Paused && !Tutorial.Running)
        {
            if (UI.Zoomed)
            {
                this.UnZoomCard();
            }
            if (!this.skip)
            {
                this.skip = true;
                base.StopAllCoroutines();
                LeanTween.cancelAll(false);
                this.TextAnimator.SetTrigger("Exit");
                this.CardAnimator.SetTrigger("Reset");
                this.ContinueButtonAnimator.SetTrigger("CloseBox");
                for (int i = 0; i < this.WildcardBoxes.Length; i++)
                {
                    GuiPanelScenarioWildcard wildcard = this.WildcardBoxes[i];
                    if (wildcard.Visible)
                    {
                        wildcard.Show(false, 0.3f);
                    }
                }
                int num2 = Scenario.Current.LocationCards.Count - this.cardList.Count;
                for (int j = 0; j < num2; j++)
                {
                    Card item = this.CreateCard(fakeCard, 0);
                    if (item != null)
                    {
                        this.cardList.Add(item);
                        item.Show(CardSideType.Back);
                        item.transform.localScale = new Vector3(0.15f, 0.15f, 1f);
                        item.transform.position = Vector3.zero;
                    }
                }
                base.StartCoroutine(this.ShowCloseScreen());
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        if (this.tapRecognizer != null)
        {
            this.tapRecognizer.enabled = !isPaused;
        }
    }

    private void ScreenController()
    {
        if (this.screenNumber <= 0)
        {
            base.StartCoroutine(this.ShowScenarioScreen());
            if (!string.IsNullOrEmpty(Scenario.Current.Villain))
            {
                this.screenNumber++;
            }
            else
            {
                this.screenNumber += 2;
            }
        }
        else if (this.screenNumber == 1)
        {
            base.StartCoroutine(this.ShowVillainScreen());
        }
        else if (this.screenNumber == 2)
        {
            if (!this.doneHenchmen)
            {
                base.StartCoroutine(this.ShowHenchmanScreen());
            }
            else
            {
                if (Tutorial.Running)
                {
                    base.StartCoroutine(this.ShowTutorialScreen());
                }
                else
                {
                    base.StartCoroutine(this.ShowCloseScreen());
                }
                this.screenNumber++;
            }
        }
        else if (this.screenNumber == 3)
        {
            base.StartCoroutine(this.ShowCloseScreen());
            this.screenNumber++;
        }
    }

    private void SetAsideVillains()
    {
        for (int i = 0; i < this.cardList.Count; i++)
        {
            this.cardList[i].transform.SetParent(null, true);
            LeanTween.move(this.cardList[i].gameObject, this.VillainPositions[i].transform.position, 0.2f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(this.cardList[i].gameObject, this.VillainPositions[i].transform.localScale, 0.2f).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            base.GetComponent<Animator>().SetTrigger("Start");
            this.ScreenController();
        }
        this.Pause(!isVisible);
    }

    [DebuggerHidden]
    private IEnumerator ShowCloseScreen() => 
        new <ShowCloseScreen>c__Iterator6E { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator ShowHenchmanScreen() => 
        new <ShowHenchmanScreen>c__Iterator6B { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator ShowScenarioScreen() => 
        new <ShowScenarioScreen>c__Iterator68 { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator ShowTutorialScreen() => 
        new <ShowTutorialScreen>c__Iterator6C { <>f__this = this };

    [DebuggerHidden]
    private IEnumerator ShowVillainScreen() => 
        new <ShowVillainScreen>c__Iterator6A { <>f__this = this };

    private void UnZoomCard()
    {
        if (this.zoomedCard != null)
        {
            UI.Zoomed = false;
            this.zoomedCard.OnGuiZoom(false);
            this.zoomedCard = null;
        }
    }

    private void ZoomCard(Card card)
    {
        if ((card != null) && (card.transform.localScale.x < 0.75f))
        {
            UI.Zoomed = true;
            card.OnGuiZoom(true);
            this.zoomedCard = card;
        }
    }

    [CompilerGenerated]
    private sealed class <HideContinueButtons>c__Iterator69 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;
        internal GuiPanelScenarioWildcard <box>__1;
        internal int <i>__0;

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
                    this.<>f__this.CardAnimator.gameObject.SetActive(true);
                    this.<>f__this.CardAnimator.SetTrigger("Reset");
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("CloseBox");
                    this.<i>__0 = 0;
                    goto Label_00D4;

                case 1:
                    break;

                case 2:
                    this.<>f__this.CardAnimator.gameObject.SetActive(false);
                    this.$PC = -1;
                    goto Label_013A;

                default:
                    goto Label_013A;
            }
        Label_00C6:
            this.<i>__0++;
        Label_00D4:
            if (this.<i>__0 < this.<>f__this.WildcardBoxes.Length)
            {
                this.<box>__1 = this.<>f__this.WildcardBoxes[this.<i>__0];
                if (this.<box>__1.Visible)
                {
                    this.<box>__1.Show(false, 0.3f);
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    goto Label_013C;
                }
                goto Label_00C6;
            }
            this.<>f__this.TextAnimator.SetTrigger("Exit");
            this.$current = new WaitForSeconds(0.5f);
            this.$PC = 2;
            goto Label_013C;
        Label_013A:
            return false;
        Label_013C:
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
    private sealed class <LocationHilightLoop>c__Iterator6D : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GameObject <$>arrow;
        internal int <i>__1;
        internal GuiWindowScenario <window>__0;
        internal GameObject arrow;

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
                    this.<window>__0 = UI.Window as GuiWindowScenario;
                    while (true)
                    {
                        this.<i>__1 = 0;
                        while (this.<i>__1 < this.<window>__0.MapPanel.Icons.Count)
                        {
                            this.arrow.transform.position = this.<window>__0.MapPanel.Icons[this.<i>__1].transform.position;
                            this.$current = new WaitForSeconds(0.5f);
                            this.$PC = 1;
                            return true;
                        Label_008E:
                            this.<i>__1++;
                        }
                    }
                    this.$PC = -1;
                    break;

                case 1:
                    goto Label_008E;
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

    [CompilerGenerated]
    private sealed class <ShowCloseScreen>c__Iterator6E : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;
        internal Animator <anim>__16;
        internal int <cardListIndex>__8;
        internal int <cardListStart>__9;
        internal int <i>__1;
        internal int <i>__10;
        internal int <i>__3;
        internal int <i>__4;
        internal int <j>__11;
        internal int <j>__12;
        internal ScenarioMapIcon[] <locations>__7;
        internal Vector3 <mapSize>__6;
        internal int <nhp>__0;
        internal Vector3 <shuffleSize>__2;
        internal float <shuffleTime>__5;
        internal int <start>__14;
        internal int <start>__15;
        internal Vector3 <upSize>__13;

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
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Close");
                    this.<>f__this.SkipButton.Fade(false, 0.15f);
                    if (Tutorial.Running)
                    {
                        break;
                    }
                    this.<nhp>__0 = 0;
                    if (this.<>f__this.cardList.Count > 0)
                    {
                        this.<>f__this.cardList[0].Decorations.Clear();
                    }
                    this.<i>__1 = 1;
                    while (this.<i>__1 < this.<>f__this.cardList.Count)
                    {
                        this.<>f__this.cardList[this.<i>__1].transform.parent = null;
                        this.<>f__this.cardList[this.<i>__1].MoveCard(this.<>f__this.HenchmenPositions[this.<nhp>__0 % this.<>f__this.HenchmenPositions.Length].transform.position, 0.2f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                        LeanTween.scale(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.HenchmenPositions[this.<nhp>__0 % this.<>f__this.HenchmenPositions.Length].transform.localScale, 0.2f).setEase(LeanTweenType.easeOutQuad);
                        this.<>f__this.cardList[this.<i>__1].Decorations.Clear();
                        this.<nhp>__0++;
                        this.<i>__1++;
                    }
                    UI.Sound.Play(SoundEffectType.GenericCardMoved);
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 1;
                    goto Label_07DB;

                case 1:
                    break;

                case 2:
                    this.<i>__4 = 0;
                    while (this.<i>__4 < this.<>f__this.cardList.Count)
                    {
                        this.<>f__this.cardList[this.<i>__4].Show(false);
                        this.<i>__4++;
                    }
                    this.<>f__this.ShuffleAnimator.transform.localScale = this.<shuffleSize>__2;
                    this.<>f__this.ShuffleAnimator.Show(true);
                    this.<shuffleTime>__5 = this.<>f__this.ShuffleAnimator.Shuffle(this.<>f__this.cardList.Count);
                    this.$current = new WaitForSeconds(this.<shuffleTime>__5);
                    this.$PC = 3;
                    goto Label_07DB;

                case 3:
                    this.<mapSize>__6 = new Vector3(0.15f, 0.15f, 1f);
                    this.<locations>__7 = UnityEngine.Object.FindObjectsOfType<ScenarioMapIcon>();
                    this.<cardListIndex>__8 = 0;
                    this.<cardListStart>__9 = 0;
                    this.<i>__10 = 0;
                    goto Label_073C;

                case 4:
                    this.<j>__12 = 0;
                    while ((this.<j>__12 < this.<>f__this.cardList.Count) && (this.<j>__12 < this.<locations>__7.Length))
                    {
                        this.<locations>__7[this.<j>__12].Flash();
                        this.<j>__12++;
                    }
                    UI.Sound.Play(SoundEffectType.CardDroppedIntoLocationMap);
                    this.$current = new WaitForSeconds(0.1f);
                    this.$PC = 5;
                    goto Label_07DB;

                case 5:
                    this.<upSize>__13 = new Vector3(1.25f * this.<>f__this.cardList[0].transform.localScale.x, 1.25f * this.<>f__this.cardList[0].transform.localScale.y, 1f);
                    this.<start>__14 = this.<cardListStart>__9;
                    while (this.<start>__14 < this.<cardListIndex>__8)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<start>__14].gameObject, this.<upSize>__13, 0.25f).setEase(LeanTweenType.easeOutQuad);
                        this.<start>__14++;
                    }
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 6;
                    goto Label_07DB;

                case 6:
                    this.<start>__15 = this.<cardListStart>__9;
                    while (this.<start>__15 < this.<cardListIndex>__8)
                    {
                        LeanTween.scale(this.<>f__this.cardList[this.<start>__15].gameObject, Vector3.zero, 0.2f).setEase(LeanTweenType.easeOutQuad);
                        this.<start>__15++;
                    }
                    this.$current = new WaitForSeconds(0.2f);
                    this.$PC = 7;
                    goto Label_07DB;

                case 7:
                    this.<cardListStart>__9 = this.<cardListIndex>__8;
                    this.<i>__10++;
                    goto Label_073C;

                case 8:
                    this.<anim>__16 = this.<>f__this.GetComponent<Animator>();
                    this.<anim>__16.SetTrigger("End");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 9;
                    goto Label_07DB;

                case 9:
                    this.<>f__this.Close();
                    UI.Busy = false;
                    this.$PC = -1;
                    goto Label_07D9;

                default:
                    goto Label_07D9;
            }
            this.<shuffleSize>__2 = new Vector3(0.4f, 0.4f, 1f);
            this.<i>__3 = 0;
            while (this.<i>__3 < this.<>f__this.cardList.Count)
            {
                this.<>f__this.cardList[this.<i>__3].Show(CardSideType.Back);
                this.<>f__this.cardList[this.<i>__3].MoveCard(Vector3.zero, 0.2f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                LeanTween.scale(this.<>f__this.cardList[this.<i>__3].gameObject, this.<shuffleSize>__2, 0.2f).setEase(LeanTweenType.easeOutQuad);
                this.<i>__3++;
            }
            UI.Sound.Play(SoundEffectType.GenericCardMoved);
            this.$current = new WaitForSeconds(0.2f);
            this.$PC = 2;
            goto Label_07DB;
        Label_073C:
            if (this.<i>__10 < Mathf.CeilToInt(((float) this.<>f__this.cardList.Count) / ((float) this.<locations>__7.Length)))
            {
                if (this.<i>__10 >= (Mathf.CeilToInt(((float) this.<>f__this.cardList.Count) / ((float) this.<locations>__7.Length)) - 1))
                {
                    this.<>f__this.ShuffleAnimator.Show(false);
                }
                this.<j>__11 = 0;
                while ((this.<j>__11 < this.<locations>__7.Length) && (this.<cardListIndex>__8 < this.<>f__this.cardList.Count))
                {
                    this.<>f__this.cardList[this.<cardListIndex>__8].Show(true);
                    this.<>f__this.cardList[this.<cardListIndex>__8].MoveCard(this.<locations>__7[this.<j>__11].transform.position, 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                    LeanTween.scale(this.<>f__this.cardList[this.<cardListIndex>__8].gameObject, this.<mapSize>__6, 0.25f).setEase(LeanTweenType.easeOutQuad);
                    this.<cardListIndex>__8++;
                    this.<j>__11++;
                }
                UI.Sound.Play(SoundEffectType.GenericCardMoved);
                this.$current = new WaitForSeconds(0.25f);
                this.$PC = 4;
            }
            else
            {
                this.$current = new WaitForSeconds(0.4f);
                this.$PC = 8;
            }
            goto Label_07DB;
        Label_07D9:
            return false;
        Label_07DB:
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
    private sealed class <ShowHenchmanScreen>c__Iterator6B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;
        internal int <i>__1;
        internal int <maxHenchmen>__3;
        internal int <nhp>__0;
        internal int <numCardsCreated>__2;

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
                    this.<>f__this.doneHenchmen = false;
                    if (this.<>f__this.arrayNumber != 0)
                    {
                        this.<nhp>__0 = 0;
                        this.<i>__1 = !this.<>f__this.IsVillainHere() ? 0 : Scenario.Current.Villains.Length;
                        while (this.<i>__1 < this.<>f__this.cardList.Count)
                        {
                            this.<>f__this.cardList[this.<i>__1].transform.parent = null;
                            this.<>f__this.cardList[this.<i>__1].MoveCard(this.<>f__this.HenchmenPositions[this.<nhp>__0].transform.position, 0.2f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                            LeanTween.scale(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.HenchmenPositions[this.<nhp>__0].transform.localScale, 0.2f).setEase(LeanTweenType.easeOutQuad);
                            while ((this.<i>__1 < (this.<>f__this.cardList.Count - 1)) && (this.<>f__this.cardList[this.<i>__1].ID == this.<>f__this.cardList[this.<i>__1 + 1].ID))
                            {
                                this.<i>__1++;
                                this.<>f__this.cardList[this.<i>__1].transform.parent = null;
                                this.<>f__this.cardList[this.<i>__1].MoveCard(this.<>f__this.HenchmenPositions[this.<nhp>__0].transform.position, 0.2f).setEase(LeanTweenType.easeOutQuad);
                                LeanTween.scale(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.HenchmenPositions[this.<nhp>__0].transform.localScale, 0.2f).setEase(LeanTweenType.easeOutQuad);
                            }
                            this.<nhp>__0++;
                            this.<i>__1++;
                        }
                        this.<>f__this.ContinueButtonAnimator.SetTrigger("Close");
                        this.<>f__this.CardAnimator.gameObject.SetActive(true);
                        this.<>f__this.CardAnimator.SetTrigger("Reset");
                        this.$current = new WaitForSeconds(0.2f);
                        this.$PC = 3;
                        goto Label_048F;
                    }
                    if (!this.<>f__this.IsVillainHere())
                    {
                        this.$current = this.<>f__this.StartCoroutine(this.<>f__this.HideContinueButtons());
                        this.$PC = 1;
                        goto Label_048F;
                    }
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Close");
                    this.<>f__this.SetAsideVillains();
                    break;

                case 1:
                    break;

                case 2:
                case 3:
                    this.<>f__this.CardAnimator.gameObject.SetActive(false);
                    this.<numCardsCreated>__2 = this.<>f__this.AddHenchmanToList(this.<>f__this.arrayNumber);
                    Tutorial.Notify(TutorialEventType.ScenarioSetupHenchmen);
                    UI.Sound.Play(this.<>f__this.GetCardFocusSound(CardType.Henchman));
                    this.<>f__this.CardAnimator.gameObject.SetActive(true);
                    this.<>f__this.CardAnimator.SetTrigger("Start");
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 4;
                    goto Label_048F;

                case 4:
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Open");
                    this.<>f__this.arrayNumber += this.<numCardsCreated>__2;
                    this.<maxHenchmen>__3 = this.<>f__this.MaxHenchmen();
                    if (this.<>f__this.arrayNumber >= this.<maxHenchmen>__3)
                    {
                        this.<>f__this.doneHenchmen = true;
                    }
                    UI.Busy = false;
                    this.$PC = -1;
                    goto Label_048D;

                default:
                    goto Label_048D;
            }
            this.<>f__this.CardAnimator.gameObject.SetActive(true);
            this.<>f__this.CardAnimator.SetTrigger("Reset");
            this.$current = new WaitForSeconds(0.2f);
            this.$PC = 2;
            goto Label_048F;
        Label_048D:
            return false;
        Label_048F:
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
    private sealed class <ShowScenarioScreen>c__Iterator68 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;
        internal GuiPanelScenarioWildcard <box>__5;
        internal int <c>__2;
        internal string <descriptionText>__0;
        internal int <i>__4;
        internal bool <isLongText>__1;
        internal string[] <powers>__3;

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
                    Tutorial.Notify(TutorialEventType.ScenarioSetupPower);
                    if (!Tutorial.Running)
                    {
                        this.<>f__this.SkipButton.Show(true);
                    }
                    this.<descriptionText>__0 = this.<>f__this.GetDescriptionText();
                    this.<isLongText>__1 = this.<descriptionText>__0.Length > 500;
                    this.<>f__this.DescriptionLabel.Text = this.<descriptionText>__0;
                    this.<>f__this.DescriptionLargeBoxArt.SetActive(this.<isLongText>__1);
                    this.<>f__this.DescriptionSmallBoxArt.SetActive(!this.<isLongText>__1);
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Start");
                    this.<>f__this.TextAnimator.SetTrigger("Enter");
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    goto Label_01C9;

                case 1:
                    this.<c>__2 = 0;
                    this.<powers>__3 = this.<>f__this.GetRandomPowerText();
                    this.<i>__4 = 0;
                    goto Label_01A7;

                case 2:
                    break;

                default:
                    goto Label_01C7;
            }
        Label_0199:
            this.<i>__4++;
        Label_01A7:
            if (this.<i>__4 < this.<powers>__3.Length)
            {
                if (this.<c>__2 < this.<>f__this.WildcardBoxes.Length)
                {
                    this.<box>__5 = this.<>f__this.WildcardBoxes[this.<c>__2++];
                    this.<box>__5.Show(this.<powers>__3[this.<i>__4], 0.5f);
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 2;
                    goto Label_01C9;
                }
                goto Label_0199;
            }
            UI.Busy = false;
            this.$PC = -1;
        Label_01C7:
            return false;
        Label_01C9:
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
    private sealed class <ShowTutorialScreen>c__Iterator6C : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;
        internal GameObject <arrow>__3;
        internal GameObject <arrowPrefab>__4;
        internal Card <bane1>__9;
        internal Card <bane2>__10;
        internal Card <bane3>__11;
        internal Card <boon1>__15;
        internal Card <boon2>__16;
        internal Card <boon3>__17;
        internal GameObject <go>__14;
        internal IEnumerator <highlightLoop>__2;
        internal int <i>__0;
        internal int <i>__1;
        internal int <i>__18;
        internal int <i>__8;
        internal Animator <tutorialAnimator>__12;
        internal GameObject <tutorialAnimatorPrefab>__13;
        internal GameObject <vfxLocationHighlight>__7;
        internal GameObject <vfxPrefab>__6;
        internal GuiWindowScenario <window>__5;

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
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Close");
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.cardList.Count)
                    {
                        this.<>f__this.cardList[this.<i>__0].transform.parent = null;
                        this.<i>__0++;
                    }
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.cardList.Count)
                    {
                        if (this.<i>__1 == 0)
                        {
                            LeanTween.moveLocal(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.VillainStowPosition.transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                            LeanTween.scale(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.VillainStowPosition.transform.localScale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                        }
                        else
                        {
                            LeanTween.moveLocal(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.HenchmenStowPosition.transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                            LeanTween.scale(this.<>f__this.cardList[this.<i>__1].gameObject, this.<>f__this.HenchmenStowPosition.transform.localScale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                        }
                        this.<i>__1++;
                    }
                    this.$current = new WaitForSeconds(0.3f);
                    this.$PC = 1;
                    goto Label_09F7;

                case 1:
                    this.<highlightLoop>__2 = null;
                    this.<arrow>__3 = null;
                    this.<arrowPrefab>__4 = Resources.Load<GameObject>("Art/VFX/vfx_Tut_Arrow");
                    if (this.<arrowPrefab>__4 != null)
                    {
                        this.<arrow>__3 = UnityEngine.Object.Instantiate<GameObject>(this.<arrowPrefab>__4);
                        if (this.<arrow>__3 != null)
                        {
                            this.<highlightLoop>__2 = this.<>f__this.LocationHilightLoop(this.<arrow>__3);
                            this.<>f__this.StartCoroutine(this.<highlightLoop>__2);
                        }
                    }
                    this.$current = this.<>f__this.StartCoroutine(Tutorial.WaitForOverlay(0x40, 0.5f, 0.3f));
                    this.$PC = 2;
                    goto Label_09F7;

                case 2:
                    if (this.<arrow>__3 != null)
                    {
                        this.<>f__this.StopCoroutine(this.<highlightLoop>__2);
                        UnityEngine.Object.Destroy(this.<arrow>__3);
                    }
                    this.<window>__5 = UI.Window as GuiWindowScenario;
                    this.<vfxPrefab>__6 = Resources.Load<GameObject>("Art/VFX/vfx_Highlight location");
                    this.<vfxLocationHighlight>__7 = null;
                    if (this.<vfxPrefab>__6 != null)
                    {
                        this.<i>__8 = 0;
                        while (this.<i>__8 < this.<window>__5.MapPanel.Icons.Count)
                        {
                            if (this.<window>__5.MapPanel.Icons[this.<i>__8].ID == "LO1B_Farmhouse")
                            {
                                this.<vfxLocationHighlight>__7 = UnityEngine.Object.Instantiate<GameObject>(this.<vfxPrefab>__6);
                                if (this.<vfxLocationHighlight>__7 != null)
                                {
                                    this.<vfxLocationHighlight>__7.transform.position = this.<window>__5.MapPanel.Icons[this.<i>__8].transform.position;
                                }
                                break;
                            }
                            this.<i>__8++;
                        }
                    }
                    break;

                case 3:
                    this.$current = this.<>f__this.StartCoroutine(Tutorial.WaitForOverlay("Tutorial_Popup_CardTypes"));
                    this.$PC = 4;
                    goto Label_09F7;

                case 4:
                    if (this.<tutorialAnimator>__12 == null)
                    {
                        goto Label_0659;
                    }
                    this.<tutorialAnimator>__12.SetTrigger("Next");
                    this.$current = new WaitForSeconds(1.5f);
                    this.$PC = 5;
                    goto Label_09F7;

                case 5:
                    this.$current = this.<>f__this.StartCoroutine(Tutorial.WaitForOverlay(0x42, 0.5f, 0.3f));
                    this.$PC = 6;
                    goto Label_09F7;

                case 6:
                    goto Label_0659;

                case 7:
                    this.<boon1>__15 = CardTable.Create("IT1B_Crowbar");
                    this.<boon1>__15.Show(CardSideType.Front);
                    this.<boon2>__16 = CardTable.Create("IT1B_PotionOfHealing");
                    this.<boon2>__16.Show(CardSideType.Front);
                    this.<boon3>__17 = CardTable.Create("IT1B_Codex");
                    this.<boon3>__17.Show(CardSideType.Front);
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back1").gameObject, this.<boon1>__15.gameObject);
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back2").gameObject, this.<boon2>__16.gameObject);
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back3").gameObject, this.<boon3>__17.gameObject);
                    this.<bane1>__9.Show(false);
                    this.<bane2>__10.Show(false);
                    this.<bane3>__11.Show(false);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 8;
                    goto Label_09F7;

                case 8:
                    this.$current = this.<>f__this.StartCoroutine(Tutorial.WaitForOverlay(0x43, 0.5f, 0.3f));
                    this.$PC = 9;
                    goto Label_09F7;

                case 9:
                    goto Label_07E6;

                case 10:
                    goto Label_0824;

                case 11:
                    this.<>f__this.ContinueButtonAnimator.SetTrigger("Open");
                    Tutorial.Message(0x44, 0.5f, 0.3f);
                    UI.Busy = false;
                    this.$PC = -1;
                    goto Label_09F5;

                default:
                    goto Label_09F5;
            }
            this.<bane1>__9 = CardTable.Create("MO1B_Ogre");
            this.<bane1>__9.Show(CardSideType.Front);
            this.<bane2>__10 = CardTable.Create("MO1B_Xulgath");
            this.<bane2>__10.Show(CardSideType.Front);
            this.<bane3>__11 = CardTable.Create("MO1B_Warlord");
            this.<bane3>__11.Show(CardSideType.Front);
            this.<tutorialAnimator>__12 = null;
            this.<tutorialAnimatorPrefab>__13 = Resources.Load<GameObject>("Art/VFX/vfx_Tut_LocTop_Highlight");
            if (this.<tutorialAnimatorPrefab>__13 != null)
            {
                this.<go>__14 = UnityEngine.Object.Instantiate<GameObject>(this.<tutorialAnimatorPrefab>__13);
                if (this.<go>__14 != null)
                {
                    this.<tutorialAnimator>__12 = this.<go>__14.GetComponent<Animator>();
                    this.<tutorialAnimator>__12.enabled = true;
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back1").gameObject, this.<bane1>__9.gameObject);
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back2").gameObject, this.<bane2>__10.gameObject);
                    Geometry.AddChild(this.<tutorialAnimator>__12.gameObject.transform.Find("card_back3").gameObject, this.<bane3>__11.gameObject);
                    this.<tutorialAnimator>__12.transform.Find("card_back1").GetComponent<SpriteRenderer>().enabled = false;
                    this.<tutorialAnimator>__12.transform.Find("card_back2").GetComponent<SpriteRenderer>().enabled = false;
                    this.<tutorialAnimator>__12.transform.Find("card_back3").GetComponent<SpriteRenderer>().enabled = false;
                }
            }
            this.$current = this.<>f__this.StartCoroutine(Tutorial.WaitForOverlay(0x41, 0.5f, 0.3f));
            this.$PC = 3;
            goto Label_09F7;
        Label_0659:
            if (this.<tutorialAnimator>__12 != null)
            {
                this.<tutorialAnimator>__12.SetTrigger("Next");
                this.$current = new WaitForSeconds(1.75f);
                this.$PC = 7;
                goto Label_09F7;
            }
        Label_07E6:
            if (this.<tutorialAnimator>__12 != null)
            {
                this.<tutorialAnimator>__12.SetTrigger("Next");
                this.$current = new WaitForSeconds(3f);
                this.$PC = 10;
                goto Label_09F7;
            }
        Label_0824:
            if (this.<vfxLocationHighlight>__7 != null)
            {
                UnityEngine.Object.Destroy(this.<vfxLocationHighlight>__7);
            }
            this.<i>__18 = 0;
            while (this.<i>__18 < this.<>f__this.cardList.Count)
            {
                if (this.<i>__18 == 0)
                {
                    LeanTween.moveLocal(this.<>f__this.cardList[this.<i>__18].gameObject, this.<>f__this.VillainPositions[this.<i>__18].transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.<>f__this.cardList[this.<i>__18].gameObject, this.<>f__this.VillainPositions[this.<i>__18].transform.localScale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                }
                else
                {
                    LeanTween.moveLocal(this.<>f__this.cardList[this.<i>__18].gameObject, this.<>f__this.HenchmenPositions[this.<i>__18].transform.position, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                    LeanTween.scale(this.<>f__this.cardList[this.<i>__18].gameObject, this.<>f__this.HenchmenPositions[this.<i>__18].transform.localScale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                }
                this.<i>__18++;
            }
            this.$current = new WaitForSeconds(0.3f);
            this.$PC = 11;
            goto Label_09F7;
        Label_09F5:
            return false;
        Label_09F7:
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
    private sealed class <ShowVillainScreen>c__Iterator6A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelScenarioPreview <>f__this;

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
                    this.<>f__this.SetAsideVillains();
                    if (this.<>f__this.cardList.Count <= 0)
                    {
                        break;
                    }
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 1;
                    goto Label_01BE;

                case 1:
                    break;

                case 2:
                    if (this.<>f__this.arrayNumber < Scenario.Current.Villains.Length)
                    {
                        this.<>f__this.cardList.Add(this.<>f__this.CreateCard(Scenario.Current.Villains[this.<>f__this.arrayNumber], 0));
                        this.<>f__this.arrayNumber++;
                        Tutorial.Notify(TutorialEventType.ScenarioSetupVillain);
                        UI.Sound.Play(this.<>f__this.GetCardFocusSound(CardType.Villain));
                        this.<>f__this.CardAnimator.gameObject.SetActive(true);
                        this.<>f__this.CardAnimator.SetTrigger("Start");
                        this.$current = new WaitForSeconds(1f);
                        this.$PC = 3;
                        goto Label_01BE;
                    }
                    goto Label_015A;

                case 3:
                    goto Label_015A;

                default:
                    goto Label_01BC;
            }
            this.$current = this.<>f__this.StartCoroutine(this.<>f__this.HideContinueButtons());
            this.$PC = 2;
            goto Label_01BE;
        Label_015A:
            this.<>f__this.ContinueButtonAnimator.SetTrigger("Open");
            if (this.<>f__this.cardList.Count == Scenario.Current.Villains.Length)
            {
                this.<>f__this.arrayNumber = 0;
                this.<>f__this.screenNumber++;
            }
            UI.Busy = false;
            this.$PC = -1;
        Label_01BC:
            return false;
        Label_01BE:
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

