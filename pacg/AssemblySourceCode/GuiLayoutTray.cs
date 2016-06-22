using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutTray : GuiLayout
{
    [Tooltip("reference to background art contained in our hierarchy")]
    public GameObject Background;
    private int cardLayoutCount;
    private int cardLayoutIndex;
    [Tooltip("button that closes this tray if you aren't forced into this tray")]
    public GuiButton CloseButton;
    [Tooltip("can tap off to close when true")]
    public bool CloseOnTap = true;
    [Tooltip("pointer to the \"card count\" prefab")]
    public GameObject counterPrefab;
    private List<GuiLayoutTrayCounter> counters;
    private float currentDeltaTranslation;
    private const float DELTA_SOUND_THRESHOLD = 0.8f;
    private Card draggedCard;
    private Vector2 dragLocation;
    private TKPanRecognizer dragRecognizer;
    private FingerGesture finger;
    private bool isClosing;
    private bool isDragging;
    private bool isOpening;
    private bool isPaused;
    private string[] originalDeckOrder;
    [Tooltip("amount of world-space to leave between each card")]
    public float padx = 0.1f;
    private TKPanRecognizer panRecognizer;
    private float scrollPosition;
    [Tooltip("should the deck be sorted before displaying?")]
    public bool Sorting;
    [Tooltip("should identical cards be drawn on top of each other?")]
    public bool Stacking;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("title explaining what this layout tray represents")]
    public GuiLabel Title;
    private Card zoomedCard;

    private Vector3 CalculateCounterPositionAt(Card card, Vector3 cardPosition)
    {
        Vector2 cardSize = this.GetCardSize(card);
        return (cardPosition + new Vector3(cardSize.x / 2f, -cardSize.y / 2f, 0f));
    }

    private void CleanupStackCounters()
    {
        for (int i = this.counters.Count - 1; i >= 0; i--)
        {
            if (!this.counters[i].Rendered)
            {
                GuiLayoutTrayCounter item = this.counters[i];
                this.counters.Remove(item);
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }

    private void Clear()
    {
        this.Filter = null;
        this.Pick = null;
        this.PickAmount = 0;
        this.PickFromHand = false;
        this.Modal = true;
        this.Margins = 0f;
        this.Deck = null;
        this.Layout = null;
        this.finger.Reset();
        this.scrollPosition = 0f;
    }

    [DebuggerHidden]
    private IEnumerator CloseCoroutine() => 
        new <CloseCoroutine>c__Iterator50 { <>f__this = this };

    private void ConfigureGestureRecognizers(bool isVisible)
    {
        if (!isVisible)
        {
            this.tapRecognizer.enabled = false;
            this.panRecognizer.enabled = false;
            this.dragRecognizer.enabled = false;
        }
        else
        {
            this.tapRecognizer.enabled = this.CloseOnTap;
            if (this.Pick == null)
            {
                this.panRecognizer.enabled = true;
                this.dragRecognizer.enabled = false;
                this.finger.Lock(FingerGesture.FingerState.Slide);
            }
            else
            {
                int num = !this.Stacking ? this.Deck.Filter(this.Filter) : this.GetNumStacks(this.Filter);
                if (num <= 8)
                {
                    this.panRecognizer.enabled = false;
                    this.dragRecognizer.enabled = true;
                    this.finger.Lock(FingerGesture.FingerState.Drag);
                }
                else
                {
                    this.panRecognizer.enabled = true;
                    this.dragRecognizer.enabled = true;
                    this.finger.Locked = false;
                }
            }
        }
    }

    private void CreateStackCounter(Card card, int number, Vector3 position)
    {
        for (int i = 0; i < this.counters.Count; i++)
        {
            if (this.counters[i].ID == card.ID)
            {
                this.counters[i].Number = number;
                return;
            }
        }
        if (number > 1)
        {
            Vector3 stackCounterPosition = this.GetStackCounterPosition(card, position);
            GameObject obj2 = UnityEngine.Object.Instantiate(this.counterPrefab, stackCounterPosition, Quaternion.identity) as GameObject;
            if (obj2 != null)
            {
                obj2.name = this.counterPrefab.name;
                obj2.transform.parent = base.transform;
                GuiLayoutTrayCounter component = obj2.GetComponent<GuiLayoutTrayCounter>();
                if (component != null)
                {
                    component.ID = card.ID;
                    component.Number = number;
                    this.counters.Add(component);
                }
            }
        }
    }

    private void DrawStackCounter(Card card, Vector3 position)
    {
        for (int i = 0; i < this.counters.Count; i++)
        {
            if (((this.counters[i].Number > 1) && (this.counters[i].ID == card.ID)) && !this.counters[i].Rendered)
            {
                Vector3 vector = this.CalculateCounterPositionAt(card, position);
                this.counters[i].transform.position = vector;
                this.counters[i].Rendered = true;
                break;
            }
        }
    }

    private Vector3 GetCardPosition(Card card, int index, int total)
    {
        float x = this.GetCardSize(card).x;
        float num2 = x + this.padx;
        float num3 = ((x * (((float) total) / 2f)) - (0.5f * x)) + (this.padx * (((float) (total - 1)) / 2f));
        Vector3 vector = base.transform.position - new Vector3(num3, 0f, 0f);
        return ((vector + new Vector3(num2 * index, 0f, 0f)) + new Vector3(this.scrollPosition, 0f, 0f));
    }

    private Vector2 GetCardSize(Card card)
    {
        Vector3 localScale = card.transform.localScale;
        card.transform.localScale = this.Scale;
        Vector2 size = card.Size;
        card.transform.localScale = localScale;
        return size;
    }

    private Card GetFirstDisplayedCard(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (this.IsCardDisplayed(deck[i]))
            {
                return deck[i];
            }
        }
        return null;
    }

    private Card GetLastDisplayedCard(Deck deck)
    {
        for (int i = deck.Count - 1; i >= 0; i--)
        {
            if (this.IsCardDisplayed(deck[i]))
            {
                return deck[i];
            }
        }
        return null;
    }

    public int GetNumStacks(CardFilter Filter)
    {
        int num = 0;
        string iD = null;
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if (this.Deck[i].ID != iD)
            {
                iD = this.Deck[i].ID;
                if ((Filter == null) || Filter.Match(this.Deck[i]))
                {
                    num++;
                }
            }
        }
        return num;
    }

    public override Vector3 GetPosition(int i)
    {
        int total = 0;
        for (int j = 0; j < this.Deck.Count; j++)
        {
            if (this.IsCardDisplayed(this.Deck[j]))
            {
                total++;
            }
        }
        return this.GetCardPosition(this.Deck[i], i, total);
    }

    private Vector3 GetStackCounterPosition(Card card, Vector3 defaultPosition)
    {
        for (int i = this.Deck.Count - 1; i >= 0; i--)
        {
            if (this.Deck[i].ID == card.ID)
            {
                return this.CalculateCounterPositionAt(card, this.Deck[i].transform.position);
            }
        }
        return defaultPosition;
    }

    private TKRect GetTrayBounds() => 
        Geometry.GetColliderScreenBounds(base.GetComponent<BoxCollider2D>());

    private void GlowCards(bool isVisible)
    {
        if (this.Pick != null)
        {
            if (isVisible)
            {
                for (int i = 0; i < this.Deck.Count; i++)
                {
                    this.Deck[i].Glow(this.Pick.Match(this.Deck[i]));
                }
                if (this.PickFromHand)
                {
                    for (int j = 0; j < Turn.Character.Hand.Count; j++)
                    {
                        Turn.Character.Hand[j].Glow(this.Pick.Match(Turn.Character.Hand[j]));
                    }
                }
            }
            else
            {
                for (int k = 0; k < this.Deck.Count; k++)
                {
                    this.Deck[k].Glow(false);
                }
                for (int m = 0; m < Turn.Character.Hand.Count; m++)
                {
                    Turn.Character.Hand[m].Glow(false);
                }
            }
        }
    }

    public void Initialize()
    {
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 1;
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.panRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.panRecognizer.zIndex = 2;
        this.panRecognizer.boundaryFrame = new TKRect?(this.GetTrayBounds());
        this.panRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if ((!UI.Busy && !UI.Zoomed) && !this.isPaused)
            {
                this.finger.Calculate(this.panRecognizer.touchLocation());
                if (this.finger.State == FingerGesture.FingerState.Slide)
                {
                    this.OnGuiPan(this.panRecognizer.deltaTranslation);
                }
            }
        };
        this.panRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (((!UI.Busy && !UI.Zoomed) && (!this.isPaused && this.isDragging)) && (this.finger.State == FingerGesture.FingerState.Slide))
            {
                this.OnGuiPanStop(this.panRecognizer.deltaTranslation);
            }
        };
        TouchKit.addGestureRecognizer(this.panRecognizer);
        this.panRecognizer.enabled = false;
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.zIndex = 2;
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if ((!UI.Busy && !UI.Zoomed) && !this.isPaused)
            {
                this.finger.Calculate(this.dragRecognizer.touchLocation());
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.dragLocation = this.dragRecognizer.touchLocation();
                    if (this.draggedCard == null)
                    {
                        this.OnGuiDragStart(this.dragLocation);
                    }
                    else
                    {
                        this.OnGuiDrag(this.dragRecognizer.deltaTranslation);
                    }
                }
            }
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (((!UI.Busy && !UI.Zoomed) && (!this.isPaused && this.isDragging)) && (this.finger.State == FingerGesture.FingerState.Drag))
            {
                this.OnGuiDragEnd(this.dragLocation);
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.dragRecognizer.enabled = false;
        this.finger = new FingerGesture();
        this.finger.gestureRecognizedEvent += delegate (FingerGesture r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                if (this.finger.State == FingerGesture.FingerState.Slide)
                {
                    this.panRecognizer.enabled = true;
                    this.dragRecognizer.enabled = false;
                }
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.panRecognizer.enabled = false;
                    this.dragRecognizer.enabled = true;
                }
            }
        };
        this.counters = new List<GuiLayoutTrayCounter>();
        this.Clear();
    }

    private bool IsCardDisplayed(Card card) => 
        ((this.Filter == null) || this.Filter.Match(card));

    private bool IsDropAllowed(GuiLayout layout, Card card)
    {
        TurnStateData stateData = Turn.GetStateData();
        if (stateData != null)
        {
            for (int i = 0; i < stateData.Actions.Length; i++)
            {
                if (stateData.Actions[i] == layout.CardAction)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnGuiDrag(Vector2 deltaTranslation)
    {
        if (this.draggedCard != null)
        {
            Vector3 screenPoint = (Vector3) (Geometry.WorldToScreenPoint(this.draggedCard.transform.position) + deltaTranslation);
            this.draggedCard.transform.position = (Vector3) Geometry.ScreenToWorldPoint(screenPoint);
        }
    }

    private void OnGuiDragEnd(Vector2 touchPos)
    {
        this.isDragging = false;
        bool isValid = false;
        if (this.draggedCard != null)
        {
            GuiLayout topLayout = GuiLayout.GetTopLayout(touchPos);
            if ((topLayout != null) && this.IsDropAllowed(topLayout, this.draggedCard))
            {
                topLayout.Deck.Add(this.draggedCard, DeckPositionType.Top);
                topLayout.Refresh();
                this.Refresh();
                isValid = true;
            }
            Turn.Refresh();
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.GlowLayouts(false, this.draggedCard);
            }
            this.draggedCard.PlayedOwner = Turn.Character.ID;
            this.draggedCard.OnGuiDrop(isValid);
            this.draggedCard = null;
            if (isValid && (--this.PickAmount <= 0))
            {
                Turn.Proceed();
            }
        }
        this.finger.Reset();
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        this.isDragging = true;
        Card topCard = GuiLayout.GetTopCard(touchPos);
        if (((topCard != null) && ((topCard.Deck == this.Deck) || ((topCard.Deck == Turn.Character.Hand) && this.PickFromHand))) && ((this.Pick != null) && this.Pick.Match(topCard)))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.GlowLayouts(true, topCard);
                topCard.OnGuiDrag();
                this.draggedCard = topCard;
            }
        }
    }

    private void OnGuiPan(Vector2 deltaTranslation)
    {
        this.isDragging = true;
        Card firstDisplayedCard = this.GetFirstDisplayedCard(this.Deck);
        Card lastDisplayedCard = this.GetLastDisplayedCard(this.Deck);
        if ((firstDisplayedCard != null) && (lastDisplayedCard != null))
        {
            float x = Geometry.GetPanDistance(0f + this.Margins, firstDisplayedCard.Size.x, 1f - this.Margins, firstDisplayedCard.transform, deltaTranslation, lastDisplayedCard.transform);
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (this.IsCardDisplayed(card))
                {
                    card.transform.position += new Vector3(x, 0f, 0f);
                }
            }
            for (int j = 0; j < this.counters.Count; j++)
            {
                GuiLayoutTrayCounter counter = this.counters[j];
                counter.transform.position += new Vector3(x, 0f, 0f);
            }
            this.scrollPosition += x;
            this.PlayScrollSfx(x);
        }
    }

    private void OnGuiPanStop(Vector2 deltaTranslation)
    {
        this.isDragging = false;
        Card firstDisplayedCard = this.GetFirstDisplayedCard(this.Deck);
        Card lastDisplayedCard = this.GetLastDisplayedCard(this.Deck);
        if ((firstDisplayedCard != null) && (lastDisplayedCard != null))
        {
            float x = Geometry.GetPanStopDistance(0f + this.Margins, firstDisplayedCard.Size.x, 1f - this.Margins, firstDisplayedCard.transform, deltaTranslation, lastDisplayedCard.transform);
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card = this.Deck[i];
                if (this.IsCardDisplayed(card))
                {
                    Vector3 destination = card.transform.position + new Vector3(x, 0f, 0f);
                    if (!LeanTween.isTweening(card.gameObject))
                    {
                        card.MoveCard(destination, 0.3f, SoundEffectType.None).setEase(LeanTweenType.easeOutQuad);
                    }
                }
            }
            for (int j = 0; j < this.counters.Count; j++)
            {
                GuiLayoutTrayCounter counter = this.counters[j];
                Vector3 to = counter.transform.position + new Vector3(x, 0f, 0f);
                if (!LeanTween.isTweening(counter.gameObject))
                {
                    LeanTween.move(counter.gameObject, to, 0.3f).setEase(LeanTweenType.easeOutQuad);
                }
            }
            this.scrollPosition += x;
        }
        this.finger.Reset();
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.zoomedCard != null)
        {
            LeanTween.delayedCall(this.zoomedCard.OnGuiZoom(false) + 0.1f, new Action(this.UnZoom));
            this.zoomedCard = null;
        }
        else
        {
            TKRect trayBounds = this.GetTrayBounds();
            if (trayBounds.contains(touchPos) && !UI.Zoomed)
            {
                Card topCard = GuiLayout.GetTopCard(touchPos);
                if (topCard != null)
                {
                    for (int i = 0; i < this.Deck.Count; i++)
                    {
                        LeanTween.cancel(this.Deck[i].gameObject);
                    }
                    for (int j = 0; j < this.counters.Count; j++)
                    {
                        LeanTween.cancel(this.counters[j].gameObject);
                    }
                    UI.Zoomed = true;
                    topCard.OnGuiZoom(true);
                    this.zoomedCard = topCard;
                    return;
                }
            }
            if ((this.Pick == null) && !trayBounds.contains(touchPos))
            {
                this.Show(false);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator OpenCoroutine() => 
        new <OpenCoroutine>c__Iterator4F { <>f__this = this };

    public override void Pause(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    private void PauseMainWindow(bool isPaused)
    {
        if ((UI.Window is GuiWindowLocation) || (UI.Window is GuiWindowCreateParty))
        {
            UI.Window.Pause(isPaused);
        }
    }

    private void PlayBackgroundAnimation(string name)
    {
        if (this.Background != null)
        {
            Animator component = this.Background.GetComponent<Animator>();
            if (component != null)
            {
                component.SetInteger("BinNum", 0);
                if (this.Layout != null)
                {
                    if (this.Layout.CardAction == ActionType.Discard)
                    {
                        component.SetInteger("BinNum", 1);
                    }
                    else if (this.Layout.CardAction == ActionType.Bury)
                    {
                        component.SetInteger("BinNum", 2);
                    }
                    else if (this.Layout.CardAction == ActionType.Recharge)
                    {
                        component.SetInteger("BinNum", 3);
                    }
                }
                component.SetTrigger(name);
            }
        }
    }

    private void PlayScrollSfx(float delta)
    {
        this.currentDeltaTranslation += Mathf.Abs(delta);
        if (this.currentDeltaTranslation > 0.8f)
        {
            UI.Sound.Play(SoundEffectType.Scrolling);
            this.currentDeltaTranslation = 0f;
        }
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            this.cardLayoutCount = !this.Stacking ? this.Deck.Filter(this.Filter) : this.GetNumStacks(this.Filter);
            this.cardLayoutIndex = 0;
            if (this.Stacking)
            {
                int number = 0;
                for (int k = 0; k < this.Deck.Count; k++)
                {
                    Card card = this.Deck[k];
                    if ((card != null) && this.IsCardDisplayed(card))
                    {
                        if ((k != 0) && (card.ID != this.Deck[k - 1].ID))
                        {
                            this.CreateStackCounter(this.Deck[k - 1], number, this.GetCardPosition(this.Deck[k - 1], this.cardLayoutIndex, this.cardLayoutCount));
                            number = 0;
                        }
                        number++;
                    }
                }
                if (this.Stacking && (this.Deck.Count > 1))
                {
                    this.CreateStackCounter(this.Deck[this.Deck.Count - 1], number, this.GetCardPosition(this.Deck[this.Deck.Count - 1], this.cardLayoutIndex, this.cardLayoutCount));
                }
            }
            for (int i = 0; i < this.counters.Count; i++)
            {
                this.counters[i].Rendered = false;
            }
            for (int j = 0; j < this.Deck.Count; j++)
            {
                Card card2 = this.Deck[j];
                if ((card2 != null) && this.IsCardDisplayed(card2))
                {
                    if ((this.Stacking && (j != 0)) && (card2.ID != this.Deck[j - 1].ID))
                    {
                        this.cardLayoutIndex++;
                    }
                    if (card2.SortingOrder < 0x1d)
                    {
                        card2.SortingOrder = 0x1d;
                    }
                    card2.Show(true);
                    if ((this.Stacking && (j >= 2)) && ((card2.ID == this.Deck[j - 1].ID) && (card2.ID == this.Deck[j - 2].ID)))
                    {
                        card2.Show(false);
                    }
                    if (base.Animations)
                    {
                        LeanTween.cancel(card2.gameObject);
                        LeanTween.scale(card2.gameObject, this.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        card2.MoveCard(this.GetCardPosition(this.Deck[j], this.cardLayoutIndex, this.cardLayoutCount), 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                        if (this.Stacking)
                        {
                            this.TweenStackCounter(card2, this.GetCardPosition(this.Deck[j], this.cardLayoutIndex, this.cardLayoutCount), 0.2f);
                        }
                    }
                    else
                    {
                        LeanTween.cancel(card2.gameObject);
                        card2.transform.localScale = this.Scale;
                        card2.transform.position = this.GetCardPosition(this.Deck[j], this.cardLayoutIndex, this.cardLayoutCount);
                        if (this.Stacking)
                        {
                            this.DrawStackCounter(card2, this.GetCardPosition(this.Deck[j], this.cardLayoutIndex, this.cardLayoutCount));
                        }
                    }
                    if (!this.Stacking)
                    {
                        this.cardLayoutIndex++;
                    }
                }
            }
            this.CleanupStackCounters();
            LeanTween.delayedCall(0.25f, new Action(this.ResetSortingOrder));
        }
    }

    public void Reset()
    {
        this.scrollPosition = 0f;
    }

    private void ResetSortingOrder()
    {
        for (int i = 0; (this.Deck != null) && (i < this.Deck.Count); i++)
        {
            if (!this.Deck[i].Zoomed)
            {
                this.Deck[i].SortingOrder = 0x1d;
                this.Deck[i].ResetLastPositionAndScale();
            }
        }
    }

    public override void Show(bool isVisible)
    {
        this.ConfigureGestureRecognizers(isVisible);
        if (isVisible)
        {
            if (base.CardAction != ActionType.None)
            {
                UI.Sound.Play(SoundEffectType.GenericOpenTrayClick);
                if ((base.CardAction == ActionType.Bury) || (base.CardAction == ActionType.Banish))
                {
                    UI.Sound.Play(SoundEffectType.BuryLayoutOpen);
                }
                else
                {
                    UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
                }
            }
            base.gameObject.SetActive(true);
            this.ShowDice(false);
            Game.Instance.StartCoroutine(this.OpenCoroutine());
        }
        else if (base.Visible && !this.isClosing)
        {
            if (base.CardAction != ActionType.None)
            {
                UI.Sound.Play(SoundEffectType.GenericCloseTrayClick);
                if ((base.CardAction == ActionType.Bury) || (base.CardAction == ActionType.Banish))
                {
                    UI.Sound.Play(SoundEffectType.BuryLayoutClose);
                }
                else
                {
                    UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
                }
            }
            base.CardAction = ActionType.None;
            this.ShowDice(true);
            Game.Instance.StartCoroutine(this.CloseCoroutine());
        }
    }

    private void ShowDice(bool isVisible)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.dicePanel.Show(isVisible);
        }
    }

    private void Sort(bool isSorted)
    {
        if (this.Deck != null)
        {
            if (isSorted)
            {
                this.originalDeckOrder = this.Deck.GetCardList();
                this.Deck.Sort();
            }
            else
            {
                this.Deck.Sort(this.originalDeckOrder);
                this.originalDeckOrder = null;
            }
        }
    }

    private void TweenStackCounter(Card card, Vector3 position, float time)
    {
        for (int i = 0; i < this.counters.Count; i++)
        {
            if (((this.counters[i].Number > 1) && (this.counters[i].ID == card.ID)) && !this.counters[i].Rendered)
            {
                Vector3 to = this.CalculateCounterPositionAt(card, position);
                LeanTween.move(this.counters[i].gameObject, to, time).setEase(LeanTweenType.easeInOutQuad);
                this.counters[i].Rendered = true;
                break;
            }
        }
    }

    private void UnZoom()
    {
        UI.Zoomed = false;
    }

    public override Deck Deck
    {
        get => 
            base.myDeck;
        set
        {
            base.myDeck = value;
        }
    }

    public CardFilter Filter { get; set; }

    public GuiLayout Layout { get; set; }

    public float Margins { get; set; }

    public bool Modal { get; set; }

    public CardFilter Pick { get; set; }

    public int PickAmount { get; set; }

    public bool PickFromHand { get; set; }

    public string TitleText
    {
        get => 
            this.Title.Text;
        set
        {
            this.Title.Text = value;
        }
    }

    [CompilerGenerated]
    private sealed class <CloseCoroutine>c__Iterator50 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutTray <>f__this;
        internal Card <card>__2;
        internal int <i>__0;
        internal int <i>__1;
        internal int <i>__3;

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
                    this.<>f__this.isClosing = true;
                    this.<>f__this.PauseMainWindow(false);
                    this.<>f__this.GlowCards(false);
                    if (this.<>f__this.Sorting)
                    {
                        this.<>f__this.Sort(false);
                    }
                    this.<>f__this.GetComponent<Collider2D>().enabled = false;
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.<>f__this.counters.Count)
                    {
                        UnityEngine.Object.Destroy(this.<>f__this.counters[this.<i>__0].gameObject);
                        this.<i>__0++;
                    }
                    this.<>f__this.counters.Clear();
                    if (!this.<>f__this.Animations)
                    {
                        break;
                    }
                    this.<>f__this.PlayBackgroundAnimation("Close");
                    if (this.<>f__this.Layout != null)
                    {
                        this.<i>__1 = 0;
                        while (this.<i>__1 < this.<>f__this.Deck.Count)
                        {
                            this.<card>__2 = this.<>f__this.Deck[this.<i>__1];
                            if (this.<card>__2 != null)
                            {
                                LeanTween.scale(this.<card>__2.gameObject, this.<>f__this.Layout.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                                this.<card>__2.MoveCard(this.<>f__this.Layout.transform.position, 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                            }
                            this.<i>__1++;
                        }
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
                    this.$PC = 1;
                    goto Label_0326;

                case 1:
                    break;

                case 2:
                    goto Label_02BD;

                default:
                    goto Label_0324;
            }
            if (this.<>f__this.Deck != null)
            {
                this.<i>__3 = 0;
                while (this.<i>__3 < this.<>f__this.Deck.Count)
                {
                    this.<>f__this.Deck[this.<i>__3].Side = CardSideType.Front;
                    this.<>f__this.Deck[this.<i>__3].Show(false);
                    this.<i>__3++;
                }
            }
            if (!this.<>f__this.Animations)
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
                this.$PC = 2;
                goto Label_0326;
            }
        Label_02BD:
            if (this.<>f__this.Background != null)
            {
                this.<>f__this.Background.SetActive(false);
            }
            this.<>f__this.Show(false);
            UI.Busy = false;
            if (!this.<>f__this.isOpening)
            {
                this.<>f__this.Clear();
            }
            this.<>f__this.isClosing = false;
            this.$PC = -1;
        Label_0324:
            return false;
        Label_0326:
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
    private sealed class <OpenCoroutine>c__Iterator4F : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutTray <>f__this;
        internal Card <card>__1;
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
                    this.<>f__this.isOpening = true;
                    break;

                case 1:
                    break;

                case 2:
                    goto Label_0122;

                default:
                    goto Label_0206;
            }
            if (this.<>f__this.isClosing)
            {
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                goto Label_0208;
            }
            if (this.<>f__this.Sorting)
            {
                this.<>f__this.Sort(true);
            }
            this.<>f__this.GlowCards(true);
            this.<>f__this.GetComponent<Collider2D>().enabled = true;
            this.<>f__this.Show(true);
            if (this.<>f__this.Modal)
            {
                this.<>f__this.PauseMainWindow(true);
                UI.Busy = true;
            }
            if (this.<>f__this.Background != null)
            {
                this.<>f__this.Background.SetActive(true);
            }
            if (this.<>f__this.Animations)
            {
                this.<>f__this.PlayBackgroundAnimation("Open");
                this.$current = new WaitForEndOfFrame();
                this.$PC = 2;
                goto Label_0208;
            }
        Label_0122:
            if (this.<>f__this.CloseButton != null)
            {
                this.<>f__this.CloseButton.Show(this.<>f__this.Pick == null);
            }
            UI.Busy = false;
            this.<>f__this.isOpening = false;
            this.<i>__0 = 0;
            while (this.<i>__0 < this.<>f__this.Deck.Count)
            {
                this.<card>__1 = this.<>f__this.Deck[this.<i>__0];
                if (this.<>f__this.Layout != null)
                {
                    this.<card>__1.transform.position = this.<>f__this.Layout.transform.position;
                }
                this.<i>__0++;
            }
            this.<>f__this.Refresh();
            this.$PC = -1;
        Label_0206:
            return false;
        Label_0208:
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

