using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiLayoutExamine : GuiLayout
{
    [Tooltip("reference to the \"action\" button in our hierarchy")]
    public GuiButton ActionButton;
    [Tooltip("reference to the third \"action\" button in our hierarchy")]
    public GuiButton AlternateButton;
    [Tooltip("reference to the prefab animator")]
    public UnityEngine.Animator Animator;
    [Tooltip("reference to background art contained in our hierarchy")]
    public GameObject Background;
    private readonly Vector3 botOffset = new Vector3(-4f, 0f, 0f);
    private int botRevealed;
    [Tooltip("reference to the \"close\" button in our hierarchy")]
    public GuiButton CloseButton;
    private Card draggedCard;
    private Vector2 dragLocation;
    private bool isRevealInProgress;
    private bool isScriptInProgress;
    private DeckType myDeckType;
    private readonly float padc = 0.5f;
    private readonly float padx = 0.1f;
    private readonly float pady = 2.5f;
    private TKPanRecognizer panRecognizer;
    private TKTapRecognizer tapRecognizer;
    private readonly Vector3 topOffset = new Vector3(4f, 0f, 0f);
    private int topRevealed;
    private Card zoomedCard;

    public void Clear()
    {
        this.Mode = ExamineModeType.None;
        this.RevealPosition = DeckPositionType.Top;
        this.Curve = false;
        this.Scroll = false;
        this.ModifyTop = false;
        this.ModifyBottom = false;
        this.Group = false;
        this.Number = 0;
        this.Top = 0;
        this.Bottom = 0;
        this.TopRevealed = 0;
        this.BottomRevealed = 0;
        this.Sort = null;
        this.Finish = false;
        this.Shuffle = false;
        this.Action = ExamineActionType.None;
        this.AlternateAction = ExamineActionType.None;
        this.Choose = null;
        this.AcquireDestination = DeckType.Hand;
        this.AcquirePosition = DeckPositionType.None;
        this.RevealCallback = null;
        this.DoneCallback = null;
        this.ActionCallback = null;
        this.CloseCallback = null;
        this.ActionButtonText = null;
        this.Script = null;
        this.draggedCard = null;
        this.zoomedCard = null;
        this.isRevealInProgress = false;
        this.isScriptInProgress = false;
        this.Source = DeckType.None;
        this.myDeckType = DeckType.None;
    }

    public float Close()
    {
        this.Choose = null;
        this.UnZoomCard();
        Game.Instance.StartCoroutine(this.CloseCoroutine());
        if (base.Animations && this.Shuffle)
        {
            return 2.5f;
        }
        if (base.Animations)
        {
            return 0.75f;
        }
        return 0f;
    }

    [DebuggerHidden]
    private IEnumerator CloseCoroutine() => 
        new <CloseCoroutine>c__Iterator47 { <>f__this = this };

    private string GetButtonText()
    {
        if (!string.IsNullOrEmpty(this.ActionButtonText))
        {
            return this.ActionButtonText;
        }
        return UI.Text(0x1b9);
    }

    private Vector3 GetCardPosition(Card card, int index, int total)
    {
        if (this.Mode == ExamineModeType.All)
        {
            float cardWidth = this.GetCardWidth(card, total);
            float num2 = cardWidth + this.padx;
            float num3 = ((cardWidth * (((float) total) / 2f)) - (0.5f * cardWidth)) + (this.padx * (((float) (total - 1)) / 2f));
            Vector3 vector = base.transform.position + new Vector3(num3, 0f, 0f);
            return (vector - new Vector3(num2 * index, 0f, 0f));
        }
        if (card.Revealed)
        {
            float num4 = this.GetCardWidth(card, total);
            float num5 = num4 + this.padx;
            float num6 = ((num4 * (((float) total) / 2f)) - (0.5f * num4)) + (this.padx * (((float) (total - 1)) / 2f));
            Vector3 vector2 = base.transform.position + new Vector3(num6, 0f, 0f);
            return (vector2 - new Vector3(num5 * index, this.pady, 0f));
        }
        if ((this.TopRevealed > 0) && (this.TopRevealed >= (index + 1)))
        {
            float num8 = this.GetCardWidth(card, total) + this.padx;
            Vector3 vector3 = base.transform.position + this.topOffset;
            return (vector3 + new Vector3(num8 * ((this.TopRevealed - 1) - index), 0f, 0f));
        }
        if ((this.BottomRevealed > 0) && (this.BottomRevealed > ((total - 1) - index)))
        {
            float num10 = this.GetCardWidth(card, total) + this.padx;
            Vector3 vector4 = base.transform.position + this.botOffset;
            return (vector4 - new Vector3(num10 * (index - (total - this.BottomRevealed)), 0f, 0f));
        }
        float padc = this.padc;
        float x = padc * (((float) total) / 2f);
        Vector3 vector5 = base.transform.position + new Vector3(x, 0f, 0f);
        return (vector5 - new Vector3(padc * index, 0f, 0f));
    }

    private Vector3 GetCardScale(Card card, int total)
    {
        if (!card.Revealed)
        {
            return this.Scale;
        }
        if (total <= 3)
        {
            return new Vector3(2f * this.Scale.x, 2f * this.Scale.y, this.Scale.z);
        }
        float num = (Game.UI.Width / ((float) total)) - this.padx;
        float x = Mathf.Min((float) ((num * card.transform.localScale.x) / card.Size.x), (float) (2f * this.Scale.x));
        return new Vector3(x, x, this.Scale.z);
    }

    private CardSideType GetCardSide(Card card)
    {
        if (!card.Known && !card.Revealed)
        {
            return CardSideType.Back;
        }
        return CardSideType.Front;
    }

    private int GetCardSortingOrder(Card card, int index)
    {
        if (card.SortingOrder == Constants.SPRITE_SORTING_DRAG)
        {
            return 0x31;
        }
        if (card.Revealed)
        {
            return (0x31 - index);
        }
        return (0x27 - index);
    }

    private float GetCardWidth(Card card, int total)
    {
        Vector3 localScale = card.transform.localScale;
        card.transform.localScale = this.GetCardScale(card, total);
        float x = card.Size.x;
        card.transform.localScale = localScale;
        return x;
    }

    private int GetClosestDeckPosition(Vector2 touchPos)
    {
        int num = 0;
        float positiveInfinity = float.PositiveInfinity;
        Vector2 vector = Geometry.ScreenToWorldPoint(touchPos);
        for (int i = 0; i < this.Deck.Count; i++)
        {
            Vector2 vector2 = this.GetCardPosition(this.Deck[i], i, this.Deck.Count);
            float num4 = Mathf.Abs((float) (vector.x - vector2.x));
            if (num4 < positiveInfinity)
            {
                positiveInfinity = num4;
                num = i;
            }
        }
        return num;
    }

    private Card GetFirstDisplayedCard(Deck deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (this.IsCardVisible(deck[i]))
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
            if (this.IsCardVisible(deck[i]))
            {
                return deck[i];
            }
        }
        return null;
    }

    private TKRect GetTrayBounds() => 
        Geometry.GetColliderScreenBounds(base.GetComponent<BoxCollider2D>());

    public Card GetZoomedCard() => 
        this.zoomedCard;

    private void HideGlow()
    {
        if (this.Deck != null)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                this.Deck[i].Glow(false);
            }
        }
    }

    public void Initialize()
    {
        this.Clear();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy && (this.draggedCard == null))
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.zIndex = 15;
        this.tapRecognizer.enabled = false;
        this.panRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.panRecognizer.boundaryFrame = new TKRect?(this.GetTrayBounds());
        this.panRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                if (this.Scroll)
                {
                    this.OnGuiPan(this.panRecognizer.deltaTranslation);
                }
                else if (this.ModifyTop || this.ModifyBottom)
                {
                    this.dragLocation = this.panRecognizer.touchLocation();
                    if (this.draggedCard == null)
                    {
                        this.OnGuiDragStart(this.dragLocation);
                    }
                    else
                    {
                        this.OnGuiDrag(this.panRecognizer.deltaTranslation);
                    }
                }
            }
        };
        this.panRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                if (this.Scroll)
                {
                    this.OnGuiPanStop(this.panRecognizer.deltaTranslation);
                }
                else if (this.ModifyTop || this.ModifyBottom)
                {
                    this.OnGuiDragEnd(this.dragLocation);
                }
            }
        };
        TouchKit.addGestureRecognizer(this.panRecognizer);
        this.panRecognizer.zIndex = 20;
        this.panRecognizer.enabled = false;
        this.Show(false);
    }

    private bool IsCardVisible(Card card) => 
        (((card != null) && (card.Disposition != DispositionType.Banish)) && (card.Disposition != DispositionType.Destroy));

    private bool IsMoveValid(int from, int to)
    {
        if ((from >= 0) && (to >= 0))
        {
            if (this.ModifyTop && ((to < this.Top) || (to == 0)))
            {
                return true;
            }
            if (this.ModifyBottom && ((((this.Deck.Count - 1) - to) < this.Bottom) || (((this.Deck.Count - 1) - to) == 0)))
            {
                return true;
            }
        }
        return false;
    }

    private void MoveCard(int from, int to)
    {
        if (from != to)
        {
            bool flag = false;
            if ((from <= this.Top) && (((this.Deck.Count - 1) - to) <= this.Bottom))
            {
                flag = true;
                this.Top--;
                this.Bottom++;
            }
            bool flag2 = false;
            if ((((this.Deck.Count - 1) - from) <= this.Bottom) && (to <= this.Top))
            {
                flag2 = true;
                this.Top++;
                this.Bottom--;
            }
            this.Deck.Move(from, to);
            if (this.Group)
            {
                if (flag && (this.Top > 0))
                {
                    for (int i = 0; i < this.Top; i++)
                    {
                        this.Deck.Move(0, this.Deck.Count - 1);
                    }
                    this.Bottom += this.Top;
                    this.Top = 0;
                }
                if (flag2 && (this.Bottom > 0))
                {
                    for (int j = 0; j < this.Bottom; j++)
                    {
                        this.Deck.Move(this.Deck.Count - 1, 0);
                    }
                    this.Top += this.Bottom;
                    this.Bottom = 0;
                }
            }
        }
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
        bool isValid = false;
        if (this.draggedCard != null)
        {
            RaycastHit2D[] hitdArray = Physics2D.RaycastAll(Geometry.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
            for (int i = 0; i < hitdArray.Length; i++)
            {
                if (hitdArray[i].collider.transform.GetComponent<GuiLayoutExamine>() != null)
                {
                    int closestDeckPosition = this.GetClosestDeckPosition(touchPos);
                    int index = this.Deck.IndexOf(this.draggedCard);
                    if (this.IsMoveValid(index, closestDeckPosition))
                    {
                        this.MoveCard(index, closestDeckPosition);
                        this.Refresh();
                        isValid = true;
                    }
                    break;
                }
            }
            this.draggedCard.OnGuiDrop(isValid);
            UI.Lock(0.2f);
            this.draggedCard = null;
        }
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        Card topCard = GuiLayout.GetTopCard(touchPos);
        if ((((topCard != null) && (topCard.Deck == this.Deck)) && (topCard.Side == CardSideType.Front)) && (this.ValidTopIndex(topCard.Deck.IndexOf(topCard), this.Top) || this.ValidBotIndex(topCard.Deck.IndexOf(topCard), this.Bottom)))
        {
            topCard.OnGuiDrag();
            this.draggedCard = topCard;
        }
    }

    private void OnGuiPan(Vector2 deltaTranslation)
    {
        if (this.Deck.Count > 1)
        {
            Card firstDisplayedCard = this.GetFirstDisplayedCard(this.Deck);
            Card lastDisplayedCard = this.GetLastDisplayedCard(this.Deck);
            float x = Geometry.GetPanDistance(0f, lastDisplayedCard.Size.x, 1f, lastDisplayedCard.transform, deltaTranslation, firstDisplayedCard.transform);
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card3 = this.Deck[i];
                card3.transform.position += new Vector3(x, 0f, 0f);
            }
        }
    }

    private void OnGuiPanStop(Vector2 deltaTranslation)
    {
        if (this.Deck.Count > 1)
        {
            Card firstDisplayedCard = this.GetFirstDisplayedCard(this.Deck);
            Card lastDisplayedCard = this.GetLastDisplayedCard(this.Deck);
            float x = Geometry.GetPanStopDistance(0f, lastDisplayedCard.Size.x, 1f, lastDisplayedCard.transform, deltaTranslation, firstDisplayedCard.transform);
            for (int i = 0; i < this.Deck.Count; i++)
            {
                Card card3 = this.Deck[i];
                Vector3 destination = card3.transform.position + new Vector3(x, 0f, 0f);
                card3.MoveCard(destination, 0.3f).setEase(LeanTweenType.easeOutQuad);
            }
            UI.Lock(0.3f);
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
            Card topCard = GuiLayout.GetTopCard(touchPos);
            if (((topCard != null) && (topCard.Side == CardSideType.Front)) && !LeanTween.isTweening(topCard.gameObject))
            {
                this.ZoomCard(topCard);
            }
        }
    }

    public override void OnLoadData()
    {
        byte[] buffer;
        if (Game.GetObjectData(base.GUID, out buffer))
        {
            ByteStream bs = new ByteStream(buffer);
            if (bs != null)
            {
                bs.ReadInt();
                this.Source = (DeckType) bs.ReadInt();
                this.Mode = (ExamineModeType) bs.ReadInt();
                this.Number = bs.ReadInt();
                this.RevealPosition = (DeckPositionType) bs.ReadInt();
                this.Curve = bs.ReadBool();
                this.Scroll = bs.ReadBool();
                this.Group = bs.ReadBool();
                this.Finish = bs.ReadBool();
                this.Shuffle = bs.ReadBool();
                this.ModifyTop = bs.ReadBool();
                this.ModifyBottom = bs.ReadBool();
                this.Top = bs.ReadInt();
                this.Bottom = bs.ReadInt();
                this.TopRevealed = bs.ReadInt();
                this.BottomRevealed = bs.ReadInt();
                this.Action = (ExamineActionType) bs.ReadInt();
                this.AlternateAction = (ExamineActionType) bs.ReadInt();
                this.ActionButtonText = bs.ReadString();
                this.AcquireDestination = (DeckType) bs.ReadInt();
                this.AcquirePosition = (DeckPositionType) bs.ReadInt();
                if (bs.ReadBool())
                {
                    this.Sort = CardFilter.FromStream(bs);
                }
                if (bs.ReadBool())
                {
                    this.Choose = CardFilter.FromStream(bs);
                }
                if (bs.ReadBool())
                {
                    this.RevealCallback = TurnStateCallback.FromStream(bs);
                }
                if (bs.ReadBool())
                {
                    this.DoneCallback = TurnStateCallback.FromStream(bs);
                }
                if (bs.ReadBool())
                {
                    this.ActionCallback = TurnStateCallback.FromStream(bs);
                }
                if (bs.ReadBool())
                {
                    this.CloseCallback = TurnStateCallback.FromStream(bs);
                }
            }
        }
    }

    public override void OnSaveData()
    {
        if (!string.IsNullOrEmpty(base.GUID))
        {
            ByteStream bs = new ByteStream();
            if (bs != null)
            {
                bs.WriteInt(1);
                bs.WriteInt((int) this.Source);
                bs.WriteInt((int) this.Mode);
                bs.WriteInt(this.Number);
                bs.WriteInt((int) this.RevealPosition);
                bs.WriteBool(this.Curve);
                bs.WriteBool(this.Scroll);
                bs.WriteBool(this.Group);
                bs.WriteBool(this.Finish);
                bs.WriteBool(this.Shuffle);
                bs.WriteBool(this.ModifyTop);
                bs.WriteBool(this.ModifyBottom);
                bs.WriteInt(this.Top);
                bs.WriteInt(this.Bottom);
                bs.WriteInt(this.TopRevealed);
                bs.WriteInt(this.BottomRevealed);
                bs.WriteInt((int) this.Action);
                bs.WriteInt((int) this.AlternateAction);
                bs.WriteString(this.ActionButtonText);
                bs.WriteInt((int) this.AcquireDestination);
                bs.WriteInt((int) this.AcquirePosition);
                bs.WriteBool(this.Sort != null);
                if (this.Sort != null)
                {
                    this.Sort.ToStream(bs);
                }
                bs.WriteBool(this.Choose != null);
                if (this.Choose != null)
                {
                    this.Choose.ToStream(bs);
                }
                bs.WriteBool(this.RevealCallback != null);
                if (this.RevealCallback != null)
                {
                    this.RevealCallback.ToStream(bs);
                }
                bs.WriteBool(this.DoneCallback != null);
                if (this.DoneCallback != null)
                {
                    this.DoneCallback.ToStream(bs);
                }
                bs.WriteBool(this.ActionCallback != null);
                if (this.ActionCallback != null)
                {
                    this.ActionCallback.ToStream(bs);
                }
                bs.WriteBool(this.CloseCallback != null);
                if (this.CloseCallback != null)
                {
                    this.CloseCallback.ToStream(bs);
                }
                Game.SetObjectData(base.GUID, bs.ToArray());
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator OpenCoroutine() => 
        new <OpenCoroutine>c__Iterator48 { <>f__this = this };

    private void PauseMainWindow(bool isPaused)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.Pause(isPaused);
        }
    }

    private void PlayBackgroundAnimation(string name)
    {
        this.Animator.SetInteger("BinNum", 0);
        if (this.Layout != null)
        {
            if (this.Layout.CardAction == ActionType.Discard)
            {
                this.Animator.SetInteger("BinNum", 1);
            }
            else if (this.Layout.CardAction == ActionType.Bury)
            {
                this.Animator.SetInteger("BinNum", 2);
            }
            else if (this.Layout.CardAction == ActionType.Recharge)
            {
                this.Animator.SetInteger("BinNum", 3);
            }
        }
        this.Animator.SetTrigger(name);
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            int num = 0;
            int total = 0;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                if (this.Deck[i].Revealed)
                {
                    total++;
                }
            }
            int num4 = 0;
            for (int j = 0; j < this.Deck.Count; j++)
            {
                Card card = this.Deck[j];
                if (card != null)
                {
                    if (!this.IsCardVisible(card))
                    {
                        num4++;
                    }
                    else if (this.Mode == ExamineModeType.All)
                    {
                        card.Side = CardSideType.Front;
                        LeanTween.scale(card.gameObject, this.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        card.MoveCard(this.GetCardPosition(card, j - num4, this.Deck.Count - num4), 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                    }
                    else if (card.Revealed)
                    {
                        card.Side = CardSideType.Front;
                        LeanTween.scale(card.gameObject, this.GetCardScale(card, total), 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        card.MoveCard(this.GetCardPosition(card, num++, total), 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                    }
                    else if (this.ValidTopIndex(j, this.TopRevealed) || this.ValidBotIndex(j, this.BottomRevealed))
                    {
                        card.Side = this.GetCardSide(card);
                        LeanTween.scale(card.gameObject, this.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        if (this.Curve)
                        {
                            Vector3[] destinations = Geometry.GetCurve(card.transform.position, this.GetCardPosition(card, j - num4, this.Deck.Count - num4), 2.5f);
                            card.MoveCard(destinations, 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                        }
                        else
                        {
                            card.MoveCard(this.GetCardPosition(card, j - num4, this.Deck.Count - num4), 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                        }
                    }
                    else
                    {
                        card.Side = this.GetCardSide(card);
                        LeanTween.scale(card.gameObject, this.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        card.MoveCard(this.GetCardPosition(card, j - num4, this.Deck.Count - num4), 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                    }
                    card.SortingOrder = this.GetCardSortingOrder(card, j);
                    card.Show(this.IsCardVisible(card));
                }
            }
        }
    }

    private void Reveal()
    {
        this.Number = Mathf.Min(this.Number, this.Deck.Count);
        if ((this.RevealPosition == DeckPositionType.Top) || (this.RevealPosition == DeckPositionType.TopAndBottom))
        {
            this.TopRevealed = 0;
        }
        if ((this.RevealPosition == DeckPositionType.Bottom) || (this.RevealPosition == DeckPositionType.TopAndBottom))
        {
            this.BottomRevealed = 0;
        }
        if ((this.RevealPosition == DeckPositionType.Top) || (this.RevealPosition == DeckPositionType.TopAndBottom))
        {
            this.Top = this.Number;
        }
        if ((this.RevealPosition == DeckPositionType.Bottom) || (this.RevealPosition == DeckPositionType.TopAndBottom))
        {
            this.Bottom = this.Number;
        }
        Card[] cards = null;
        if (this.RevealPosition == DeckPositionType.Top)
        {
            cards = new Card[this.Number];
            for (int i = 0; i < this.Number; i++)
            {
                cards[i] = this.Deck[i];
            }
        }
        if (this.RevealPosition == DeckPositionType.Bottom)
        {
            cards = new Card[this.Number];
            for (int j = 0; j < this.Number; j++)
            {
                cards[j] = this.Deck[(this.Deck.Count - 1) - j];
            }
        }
        if (this.RevealPosition == DeckPositionType.TopAndBottom)
        {
            cards = new Card[Mathf.Min(this.Number * 2, this.Deck.Count)];
            int index = 0;
            while ((index < this.Number) && (index < this.Deck.Count))
            {
                cards[index] = this.Deck[index];
                index++;
            }
            while ((index < (this.Number * 2)) && ((((this.Deck.Count - 1) - index) + this.Number) >= 0))
            {
                cards[index] = this.Deck[((this.Deck.Count - 1) - index) + this.Number];
                index++;
            }
        }
        if (cards != null)
        {
            base.StartCoroutine(this.RevealCoroutine(cards));
        }
    }

    [DebuggerHidden]
    private IEnumerator RevealCoroutine(Card[] cards) => 
        new <RevealCoroutine>c__Iterator49 { 
            cards = cards,
            <$>cards = cards,
            <>f__this = this
        };

    private void RevealKnownCards()
    {
        if ((this.Deck != null) && (this.Deck.Count > 0))
        {
            this.BottomRevealed = 0;
            this.TopRevealed = 0;
            if (this.Mode != ExamineModeType.All)
            {
                if (this.Top == 0)
                {
                    int num = Mathf.Min(this.Deck.Count, Constants.NUM_CARDS_REVEALED_IN_EXAMINE);
                    for (int i = 0; i < num; i++)
                    {
                        if (this.Deck[i].Known)
                        {
                            this.TopRevealed++;
                        }
                        else
                        {
                            if (((i + 1) >= num) || !this.Deck[i + 1].Known)
                            {
                                break;
                            }
                            this.TopRevealed++;
                        }
                    }
                }
                if (this.Bottom == 0)
                {
                    int num3 = Mathf.Min(this.Deck.Count, Constants.NUM_CARDS_REVEALED_IN_EXAMINE);
                    for (int j = 0; j < num3; j++)
                    {
                        if (this.Deck[(this.Deck.Count - 1) - j].Known)
                        {
                            this.BottomRevealed++;
                        }
                        else
                        {
                            if (((j + 1) >= num3) || !this.Deck[(this.Deck.Count - 1) - j].Known)
                            {
                                break;
                            }
                            this.BottomRevealed++;
                        }
                    }
                }
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        this.panRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.PauseMainWindow(true);
            base.gameObject.SetActive(true);
            this.RevealKnownCards();
            this.ShowBackground(true);
            if (this.Mode == ExamineModeType.Reveal)
            {
                this.Reveal();
            }
            else
            {
                this.Refresh();
            }
        }
        else
        {
            this.ShowBackground(false);
            base.gameObject.SetActive(false);
            this.PauseMainWindow(false);
        }
    }

    private void ShowBackground(bool isVisible)
    {
        if (isVisible)
        {
            if (this.Background != null)
            {
                GuiPanelExamine.Open = true;
                this.Background.SetActive(true);
                this.ActionButton.Show(false);
                this.CloseButton.Show(false);
                this.AlternateButton.Show(false);
                if (base.Animations)
                {
                    Game.Instance.StartCoroutine(this.OpenCoroutine());
                }
            }
        }
        else if (this.Background != null)
        {
            this.Background.SetActive(false);
            this.ActionButton.Show(false);
            this.CloseButton.Show(false);
            this.AlternateButton.Show(false);
        }
    }

    private void ShowButton(GuiButton button, ExamineActionType type)
    {
        if (type == ExamineActionType.None)
        {
            button.Show(false);
        }
        else
        {
            button.Show(true);
            if (!string.IsNullOrEmpty(this.ActionButtonText))
            {
                button.Text = this.ActionButtonText;
            }
            else if (type == ExamineActionType.Acquire)
            {
                if (Turn.Card.IsBoon())
                {
                    button.Text = type.ToText();
                }
            }
            else
            {
                button.Text = type.ToText();
            }
        }
    }

    private void ShowGlow(bool isVisible, CardFilter filter)
    {
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if (filter.Match(this.Deck[i]))
            {
                this.Deck[i].Glow(isVisible);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator ShuffleCards() => 
        new <ShuffleCards>c__Iterator4A { <>f__this = this };

    private int SortTopCards(CardFilter filter)
    {
        int num = 0;
        int oldIndex = this.Top - 1;
        for (int i = this.Top - 1; i >= 0; i--)
        {
            if (filter.Match(this.Deck[oldIndex]))
            {
                if (this.Deck[oldIndex].GUID != Turn.EncounteredGuid)
                {
                    this.Deck.Move(oldIndex, 0);
                }
                num++;
            }
            else
            {
                this.Deck[oldIndex].Known = false;
                Vector3 to = new Vector3(this.Deck[oldIndex].transform.localScale.x * 0.9f, this.Deck[oldIndex].transform.localScale.y * 0.9f, this.Deck[oldIndex].transform.localScale.z);
                LeanTween.scale(this.Deck[oldIndex].gameObject, to, 0.15f).setRepeat(2).setLoopPingPong().setEase(LeanTweenType.easeInOutQuad);
                oldIndex--;
            }
        }
        return num;
    }

    private void UnZoomCard()
    {
        if (this.zoomedCard != null)
        {
            this.zoomedCard.OnGuiZoom(false);
            UI.Zoomed = false;
            this.zoomedCard = null;
            this.draggedCard = null;
            UI.Lock(0.2f);
            if (this.Choose != null)
            {
                this.Action = ExamineActionType.None;
                this.ShowButton(this.ActionButton, this.Action);
            }
        }
    }

    private bool ValidBotIndex(int i, int bottom) => 
        ((bottom > 0) && (bottom > ((this.Deck.Count - 1) - i)));

    private bool ValidTopIndex(int i, int top) => 
        ((top > 0) && (top >= (i + 1)));

    private void ZoomCard(Card card)
    {
        if (card != null)
        {
            UI.Zoomed = true;
            card.OnGuiZoom(true);
            this.zoomedCard = card;
            if (((this.Choose != null) && this.Choose.Match(this.zoomedCard)) && (this.Deck.IndexOf(this.zoomedCard) >= 0))
            {
                this.Action = ExamineActionType.Acquire;
                this.Deck.Move(this.Deck.IndexOf(this.zoomedCard), 0);
                this.ShowButton(this.ActionButton, this.Action);
            }
        }
    }

    public DeckType AcquireDestination { get; set; }

    public DeckPositionType AcquirePosition { get; set; }

    public ExamineActionType Action { get; set; }

    public string ActionButtonText { get; set; }

    public TurnStateCallback ActionCallback { get; set; }

    public ExamineActionType AlternateAction { get; set; }

    public int Bottom { get; set; }

    public int BottomRevealed
    {
        get => 
            (this.Bottom + this.botRevealed);
        set
        {
            this.botRevealed = value;
        }
    }

    public CardFilter Choose { get; set; }

    public TurnStateCallback CloseCallback { get; set; }

    public bool Curve { get; set; }

    public TurnStateCallback DoneCallback { get; set; }

    public bool Finish { get; set; }

    public bool Group { get; set; }

    private GuiLayout Layout { get; set; }

    public ExamineModeType Mode { get; set; }

    public bool ModifyBottom { get; set; }

    public bool ModifyTop { get; set; }

    public int Number { get; set; }

    public TurnStateCallback RevealCallback { get; set; }

    public DeckPositionType RevealPosition { get; set; }

    public GuiScript Script { get; set; }

    public bool Scroll { get; set; }

    public bool Shuffle { get; set; }

    public CardFilter Sort { get; set; }

    public DeckType Source
    {
        get => 
            this.myDeckType;
        set
        {
            this.myDeckType = value;
            this.Layout = this.myDeckType.GetLayout();
            this.Deck = this.myDeckType.GetDeck();
        }
    }

    public int Top { get; set; }

    public int TopRevealed
    {
        get => 
            (this.Top + this.topRevealed);
        set
        {
            this.topRevealed = value;
        }
    }

    [CompilerGenerated]
    private sealed class <CloseCoroutine>c__Iterator47 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutExamine <>f__this;
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
                    if (this.<>f__this.DoneCallback != null)
                    {
                        this.<>f__this.DoneCallback.Invoke();
                    }
                    if (!this.<>f__this.Animations)
                    {
                        goto Label_02F8;
                    }
                    UI.Busy = true;
                    if (!this.<>f__this.Shuffle)
                    {
                        break;
                    }
                    if (((this.<>f__this.Mode == ExamineModeType.All) || (this.<>f__this.Action == ExamineActionType.Evade)) && (this.<>f__this.Deck != null))
                    {
                        this.<i>__0 = 0;
                        while (this.<i>__0 < this.<>f__this.Deck.Count)
                        {
                            if (this.<>f__this.IsCardVisible(this.<>f__this.Deck[this.<i>__0]))
                            {
                                this.<>f__this.Deck[this.<i>__0].Show(CardSideType.Back);
                                this.<>f__this.Deck[this.<i>__0].transform.localScale = this.<>f__this.Deck[this.<i>__0].transform.localScale;
                            }
                            this.<i>__0++;
                        }
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.ShuffleCards());
                    this.$PC = 1;
                    goto Label_0433;

                case 1:
                    break;

                case 2:
                    goto Label_02F8;

                case 3:
                    goto Label_03F3;

                default:
                    goto Label_0431;
            }
            this.<>f__this.PlayBackgroundAnimation("Close");
            if (Location.Current.Closed && (Location.Current.Deck.Count == 0))
            {
                UI.Sound.Play(SoundEffectType.ClosedEmptyLocationTrayClose);
            }
            else
            {
                UI.Sound.Play(SoundEffectType.GenericLayoutTrayClose);
            }
            if ((this.<>f__this.Layout != null) && (this.<>f__this.Deck != null))
            {
                this.<i>__1 = 0;
                while (this.<i>__1 < this.<>f__this.Deck.Count)
                {
                    this.<card>__2 = this.<>f__this.Deck[this.<i>__1];
                    if ((this.<card>__2 != null) && !this.<card>__2.Revealed)
                    {
                        LeanTween.scale(this.<card>__2.gameObject, this.<>f__this.Layout.Scale, 0.25f).setEase(LeanTweenType.easeInOutQuad);
                        this.<card>__2.MoveCard(this.<>f__this.Layout.transform.position, 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                    }
                    this.<i>__1++;
                }
            }
            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
            this.$PC = 2;
            goto Label_0433;
        Label_02F8:
            if (this.<>f__this.Deck != null)
            {
                this.<i>__3 = 0;
                while (this.<i>__3 < this.<>f__this.Deck.Count)
                {
                    this.<>f__this.Deck[this.<i>__3].Side = CardSideType.Front;
                    if (!this.<>f__this.Deck[this.<i>__3].Revealed)
                    {
                        this.<>f__this.Deck[this.<i>__3].Show(false);
                    }
                    this.<>f__this.Deck[this.<i>__3].Revealed = false;
                    this.<i>__3++;
                }
            }
            if (this.<>f__this.Animations)
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.25f));
                this.$PC = 3;
                goto Label_0433;
            }
        Label_03F3:
            (UI.Window as GuiWindowLocation).powersPanel.OnExamineFinished();
            this.<>f__this.HideGlow();
            this.<>f__this.Show(false);
            GuiPanelExamine.Open = false;
            UI.Busy = false;
            this.$PC = -1;
        Label_0431:
            return false;
        Label_0433:
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
    private sealed class <OpenCoroutine>c__Iterator48 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutExamine <>f__this;

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
                    this.<>f__this.PlayBackgroundAnimation("Open");
                    UI.Sound.Play(SoundEffectType.GenericLayoutTrayOpen);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.4f));
                    this.$PC = 1;
                    return true;

                case 1:
                    if (this.<>f__this.Script != null)
                    {
                        this.<>f__this.CloseButton.Show(false);
                        this.<>f__this.ActionButton.Show(false);
                        this.<>f__this.AlternateButton.Show(false);
                        this.<>f__this.Refresh();
                        this.<>f__this.isScriptInProgress = this.<>f__this.Script.Play();
                    }
                    if (!this.<>f__this.isRevealInProgress && !this.<>f__this.isScriptInProgress)
                    {
                        this.<>f__this.CloseButton.Show(true);
                        this.<>f__this.CloseButton.Refresh();
                        UI.Busy = false;
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

    [CompilerGenerated]
    private sealed class <RevealCoroutine>c__Iterator49 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card[] <$>cards;
        internal GuiLayoutExamine <>f__this;
        internal Vector3 <dest>__7;
        internal int <i>__0;
        internal int <i>__1;
        internal int <i>__3;
        internal int <i>__5;
        internal int <i>__6;
        internal int <numVisibleCards>__4;
        internal Vector3 <v>__2;
        internal Card[] cards;

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
                    this.<>f__this.CloseButton.Show(false);
                    this.<>f__this.ActionButton.Show(false);
                    this.<>f__this.AlternateButton.Show(false);
                    this.<>f__this.isRevealInProgress = true;
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                    this.$PC = 1;
                    goto Label_0721;

                case 1:
                    this.<i>__0 = 0;
                    break;

                case 2:
                    this.<i>__0++;
                    break;

                case 3:
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.cards.Length)
                    {
                        if (this.cards[this.<i>__1].Revealed)
                        {
                            this.<v>__2 = new Vector3(this.cards[this.<i>__1].transform.localScale.x * 0.9f, this.cards[this.<i>__1].transform.localScale.y * 0.9f, this.cards[this.<i>__1].transform.localScale.z);
                            LeanTween.scale(this.cards[this.<i>__1].gameObject, this.<v>__2, 0.15f).setRepeat(2).setLoopPingPong().setEase(LeanTweenType.easeInOutQuad);
                        }
                        this.<i>__1++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 4;
                    goto Label_0721;

                case 4:
                    this.<i>__3 = 0;
                    while (this.<i>__3 < this.cards.Length)
                    {
                        this.cards[this.<i>__3].Revealed = false;
                        this.<i>__3++;
                    }
                    this.<numVisibleCards>__4 = 0;
                    this.<i>__5 = 0;
                    while (this.<i>__5 < this.cards.Length)
                    {
                        if (this.<>f__this.IsCardVisible(this.cards[this.<i>__5]))
                        {
                            this.<numVisibleCards>__4++;
                        }
                        this.<i>__5++;
                    }
                    UI.Sound.Play(SoundEffectType.CardExaminedReturn);
                    if (this.<numVisibleCards>__4 <= 3)
                    {
                        this.<>f__this.Refresh();
                        this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                        this.$PC = 5;
                        goto Label_0721;
                    }
                    if (this.<>f__this.Sort == null)
                    {
                        this.<i>__6 = 0;
                        while (this.<i>__6 < this.cards.Length)
                        {
                            this.<dest>__7 = new Vector3(this.cards[this.<i>__6].transform.position.x, this.<>f__this.transform.position.y, this.cards[this.<i>__6].transform.position.z);
                            this.cards[this.<i>__6].MoveCard(this.<dest>__7, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                            if (this.cards[this.<i>__6].transform.localScale.x > this.<>f__this.Scale.x)
                            {
                                LeanTween.scale(this.cards[this.<i>__6].gameObject, this.<>f__this.Scale, 0.3f).setEase(LeanTweenType.easeInOutQuad);
                            }
                            this.<i>__6++;
                        }
                        this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                        this.$PC = 6;
                        goto Label_0721;
                    }
                    goto Label_04FD;

                case 5:
                case 6:
                    goto Label_04FD;

                case 7:
                    this.<>f__this.Top = this.<>f__this.SortTopCards(this.<>f__this.Sort);
                    if (this.<>f__this.Shuffle)
                    {
                        this.<>f__this.BottomRevealed = 0;
                        this.<>f__this.TopRevealed = 0;
                    }
                    if (this.cards.Length > this.<>f__this.Top)
                    {
                        UI.Sound.Play(SoundEffectType.FlipCardFaceDown);
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 8;
                    goto Label_0721;

                case 8:
                    if (this.cards.Length > this.<>f__this.Top)
                    {
                        UI.Sound.Play(SoundEffectType.HorizontalCardSlideBack);
                    }
                    this.<>f__this.Refresh();
                    goto Label_0621;

                default:
                    goto Label_071F;
            }
            if (this.<i>__0 < this.cards.Length)
            {
                this.cards[this.<i>__0].Known = true;
                this.cards[this.<i>__0].Revealed = true;
                if ((this.<i>__0 == 0) && (this.cards.Length > 1))
                {
                    UI.Sound.Play(SoundEffectType.HorizontalCardFanOut);
                }
                else
                {
                    UI.Sound.Play(SoundEffectType.GenericFlickCard);
                }
                this.<>f__this.Refresh();
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.5f));
                this.$PC = 2;
            }
            else
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1.5f));
                this.$PC = 3;
            }
            goto Label_0721;
        Label_04FD:
            if (this.<>f__this.RevealCallback != null)
            {
                this.<>f__this.RevealCallback.Invoke();
            }
            if (this.<>f__this.Sort != null)
            {
                this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                this.$PC = 7;
                goto Label_0721;
            }
        Label_0621:
            if ((this.cards.Length > 0) && ((this.<>f__this.Choose == null) || this.<>f__this.Choose.Match(this.cards[0])))
            {
                this.<>f__this.ShowButton(this.<>f__this.ActionButton, this.<>f__this.Action);
                this.<>f__this.ShowButton(this.<>f__this.AlternateButton, this.<>f__this.AlternateAction);
            }
            this.<>f__this.CloseButton.Show(true);
            if (this.<>f__this.Choose != null)
            {
                this.<>f__this.ShowGlow(true, this.<>f__this.Choose);
            }
            this.<>f__this.CloseButton.Refresh();
            this.<>f__this.ActionButton.Refresh();
            this.<>f__this.AlternateButton.Refresh();
            this.<>f__this.isRevealInProgress = false;
            UI.Busy = false;
            this.$PC = -1;
        Label_071F:
            return false;
        Label_0721:
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
    private sealed class <ShuffleCards>c__Iterator4A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiLayoutExamine <>f__this;
        internal Card <card>__2;
        internal int <i>__1;
        internal int <numCards>__0;
        internal float <shuffleTime>__4;
        internal GuiWindowLocation <window>__3;

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
                    this.<numCards>__0 = 0;
                    this.<i>__1 = 0;
                    while (this.<i>__1 < this.<>f__this.Deck.Count)
                    {
                        this.<card>__2 = this.<>f__this.Deck[this.<i>__1];
                        if (this.<>f__this.IsCardVisible(this.<card>__2) && ((this.<card>__2.Side == CardSideType.Back) || (!this.<>f__this.ValidTopIndex(this.<i>__1, this.<>f__this.Top) && !this.<>f__this.ValidBotIndex(this.<i>__1, this.<>f__this.Bottom))))
                        {
                            this.<numCards>__0++;
                            this.<card>__2.MoveCard(this.<>f__this.transform.position, 0.25f, SoundEffectType.None).setEase(LeanTweenType.easeInOutQuad);
                        }
                        this.<i>__1++;
                    }
                    UI.Sound.Play(SoundEffectType.HorizontalCardUnfan);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.3f));
                    this.$PC = 1;
                    goto Label_025C;

                case 1:
                    if (this.<numCards>__0 <= 1)
                    {
                        break;
                    }
                    this.<window>__3 = UI.Window as GuiWindowLocation;
                    if (this.<window>__3 == null)
                    {
                        break;
                    }
                    this.<window>__3.shufflePanel.transform.position = this.<>f__this.transform.position;
                    this.<window>__3.shufflePanel.transform.localScale = this.<>f__this.Deck[0].transform.localScale;
                    this.<window>__3.shufflePanel.Show(true);
                    this.<window>__3.shufflePanel.SortingOrder = 100;
                    this.<shuffleTime>__4 = this.<window>__3.shufflePanel.Shuffle(this.<numCards>__0);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.<shuffleTime>__4));
                    this.$PC = 2;
                    goto Label_025C;

                case 2:
                    this.<window>__3.shufflePanel.Show(false);
                    break;

                default:
                    goto Label_025A;
            }
            this.$PC = -1;
        Label_025A:
            return false;
        Label_025C:
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

