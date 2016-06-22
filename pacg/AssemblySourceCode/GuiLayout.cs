using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class GuiLayout : MonoBehaviour
{
    [Tooltip("should this layout play its card tween animations?")]
    public bool Animations = true;
    [Tooltip("the action to inovke when a card is dropped on this layout")]
    public ActionType CardAction;
    [Tooltip("are cards animated within this bin?")]
    public bool CardAnimations;
    [Tooltip("scale of a card displayed in this layout")]
    public float CardSize = 0.5f;
    [Tooltip("reference to our label which counts a number of cards")]
    public GuiLayoutCounter Counter;
    [Tooltip("scale of a card displayed on the face of this layout")]
    public float DeckSize = 0.2f;
    [Tooltip("reference to our glow animator in the UI")]
    public GameObject GlowAnimator;
    [Tooltip("unique id used to save or load")]
    public string GUID;
    [Tooltip("reference to our hover animator in the UI")]
    public GameObject HoverAnimator;
    protected Deck myDeck;
    [Tooltip("optional reference to this layout's tray object in this scene")]
    public GuiLayoutTray Tray;

    protected GuiLayout()
    {
    }

    protected virtual void Awake()
    {
        this.Visible = true;
        this.Glow(false);
    }

    public virtual void Display()
    {
    }

    protected Deck FindDeckChild(string deckname)
    {
        Transform transform = base.transform.FindChild(deckname);
        if (transform != null)
        {
            return transform.GetComponent<Deck>();
        }
        return null;
    }

    public virtual ActionType GetActionType(Card card) => 
        this.CardAction;

    public virtual Vector3 GetPosition(int i) => 
        base.transform.position;

    public static Card GetTopCard(Vector2 touchPos)
    {
        RaycastHit2D[] hitdArray = Physics2D.RaycastAll(Geometry.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
        if (hitdArray.Length <= 0)
        {
            return null;
        }
        int index = 0;
        int sortingOrder = hitdArray[0].transform.GetComponentInChildren<SpriteRenderer>().sortingOrder;
        for (int i = 1; i < hitdArray.Length; i++)
        {
            int num4 = hitdArray[i].transform.GetComponentInChildren<SpriteRenderer>().sortingOrder;
            if (num4 > sortingOrder)
            {
                sortingOrder = num4;
                index = i;
            }
        }
        return hitdArray[index].collider.transform.parent.parent.GetComponent<Card>();
    }

    public static GuiLayout GetTopLayout(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(Geometry.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
        if (hitd != 0)
        {
            return hitd.collider.transform.GetComponent<GuiLayout>();
        }
        return null;
    }

    protected virtual TutorialEventType GetTutorialEvent()
    {
        if (this.CardAction == ActionType.Discard)
        {
            return TutorialEventType.CardDiscarded;
        }
        if (this.CardAction == ActionType.Recharge)
        {
            return TutorialEventType.CardRecharged;
        }
        if (this.CardAction == ActionType.Top)
        {
            return TutorialEventType.CardRecharged;
        }
        if (this.CardAction == ActionType.Reveal)
        {
            return TutorialEventType.CardRevealed;
        }
        if (this.CardAction == ActionType.Bury)
        {
            return TutorialEventType.CardBuried;
        }
        if (this.CardAction == ActionType.Banish)
        {
            return TutorialEventType.CardBanished;
        }
        return TutorialEventType.None;
    }

    public virtual void Glow(bool isVisible)
    {
        if (this.GlowAnimator != null)
        {
            Animator component = this.GlowAnimator.GetComponent<Animator>();
            if ((component != null) && component.gameObject.activeInHierarchy)
            {
                if (isVisible)
                {
                    component.SetBool("Glow", true);
                    if ((this.CardAnimations && (this.Deck != null)) && (this.Deck.Count > 0))
                    {
                        component.SetBool("Card", true);
                    }
                }
                else
                {
                    if (!this.CardAnimations)
                    {
                        component.SetBool("Glow", false);
                    }
                    if (this.Deck == null)
                    {
                        component.SetBool("Glow", false);
                    }
                    if ((this.CardAnimations && (this.Deck != null)) && (this.Deck.Count > 0))
                    {
                        component.SetBool("Card", true);
                    }
                    if ((this.CardAnimations && (this.Deck != null)) && (this.Deck.Count == 0))
                    {
                        component.SetBool("Card", false);
                        component.SetBool("Glow", false);
                    }
                }
            }
        }
    }

    public virtual int IndexOf(int playedPower, Card playedPowerOwner)
    {
        for (int i = 0; i < this.Deck.Count; i++)
        {
            if ((this.Deck[i].PlayedPower == playedPower) && (this.Deck[i].PlayedPowerOwner == playedPowerOwner))
            {
                return i;
            }
        }
        return -1;
    }

    public void InsertAtDropPosition(Card card)
    {
        if ((card.Deck != this.Deck) || this.IsDropReordering(card))
        {
            float maxValue = float.MaxValue;
            int newIndex = -1;
            for (int i = 0; i < this.Deck.Count; i++)
            {
                if (this.Deck[i].Visible && (this.Deck[i].GUID != card.GUID))
                {
                    float num4 = Vector3.SqrMagnitude(card.transform.position - this.Deck[i].transform.position);
                    if (num4 < maxValue)
                    {
                        maxValue = num4;
                        newIndex = i;
                    }
                }
            }
            if (card.Deck != this.Deck)
            {
                this.Deck.Add(card, DeckPositionType.Bottom);
            }
            if (newIndex >= 0)
            {
                for (int j = 0; j < this.Deck.Count; j++)
                {
                    if (this.Deck[j].GUID == card.GUID)
                    {
                        this.Deck.Move(j, newIndex);
                        break;
                    }
                }
            }
        }
    }

    public virtual bool IsDeactivateOnDrag(Card card) => 
        true;

    public virtual bool IsDeactivateOnDrop(Card card, GuiLayout layout) => 
        true;

    protected virtual bool IsDeactivationNecessary(Card card, bool allCharacters) => 
        ((card.PlayedOwner == Turn.Character.ID) || allCharacters);

    public virtual bool IsDropPossible(Card card)
    {
        if (card == null)
        {
            return false;
        }
        if (this.Deck == null)
        {
            return false;
        }
        if (card.Deck == null)
        {
            return false;
        }
        return true;
    }

    private bool IsDropReordering(Card card)
    {
        if (card.Deck == this.Deck)
        {
            RaycastHit2D[] hitdArray = Physics2D.RaycastAll(card.transform.position, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_CARD);
            if (hitdArray.Length > 0)
            {
                for (int i = 0; i < hitdArray.Length; i++)
                {
                    Card componentInParent = hitdArray[i].transform.GetComponentInParent<Card>();
                    if (((componentInParent != null) && (componentInParent.Deck == card.Deck)) && (componentInParent.GUID != card.GUID))
                    {
                        return true;
                    }
                }
            }
            if (this.Deck.Count > 0)
            {
                if (card.transform.position.x < this.Deck[0].transform.position.x)
                {
                    return true;
                }
                if (card.transform.position.x > this.Deck[this.Deck.Count - 1].transform.position.x)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsDropRestricted(Card card)
    {
        EffectCardRestrictionPending effect = Turn.Character.GetEffect(EffectType.CardRestrictionPending) as EffectCardRestrictionPending;
        return ((effect != null) && effect.Match(card));
    }

    public virtual bool OnGuiDrag(Card card) => 
        !card.Locked;

    public virtual bool OnGuiDrop(Card card) => 
        false;

    public virtual bool OnGuiHover(Card card)
    {
        if ((this.HoverAnimator != null) && Turn.IsActionAllowed(this.GetActionType(card), card))
        {
            this.HoverAnimator.SetActive(true);
            return true;
        }
        return false;
    }

    public virtual bool OnGuiHover(bool isHovering)
    {
        if (this.HoverAnimator != null)
        {
            this.HoverAnimator.SetActive(isHovering);
            return true;
        }
        return false;
    }

    public virtual void OnLoadData()
    {
        if (!string.IsNullOrEmpty(this.GUID) && (this.Deck != null))
        {
            byte[] buffer;
            if (Game.GetObjectData(this.GUID, out buffer))
            {
                ByteStream bs = new ByteStream(buffer);
                if (bs != null)
                {
                    bs.ReadInt();
                    this.Visible = bs.ReadBool();
                    this.Deck.FromStream(bs);
                }
            }
            this.Refresh();
        }
    }

    public virtual void OnSaveData()
    {
        if (!string.IsNullOrEmpty(this.GUID) && (this.Deck != null))
        {
            ByteStream bs = new ByteStream();
            if (bs != null)
            {
                bs.WriteInt(1);
                bs.WriteBool(this.Visible);
                this.Deck.ToStream(bs);
                Game.SetObjectData(this.GUID, bs.ToArray());
            }
        }
    }

    public virtual void Pause(bool isPaused)
    {
    }

    public void PlayOnCardDroppedSfx()
    {
        switch (this.CardAction)
        {
            case ActionType.Recharge:
                UI.Sound.Play(SoundEffectType.CardRecharged);
                break;

            case ActionType.Discard:
                if (Turn.State != GameStateType.Discard)
                {
                    UI.Sound.Play(SoundEffectType.CardDiscarded);
                    break;
                }
                UI.Sound.Play(SoundEffectType.TrashCanDiscard);
                break;

            case ActionType.Bury:
                UI.Sound.Play(SoundEffectType.CardBury);
                break;

            case ActionType.Banish:
                UI.Sound.Play(SoundEffectType.CardBanish);
                break;
        }
    }

    public virtual void Position(Card card)
    {
        card.transform.position = base.transform.position;
        card.transform.localScale = this.Scale;
    }

    public virtual void Refresh()
    {
    }

    public void ReturnCards(bool allCharacters)
    {
        if (this.Deck != null)
        {
            for (int i = this.Deck.Count - 1; (i >= 0) && (i < this.Deck.Count); i--)
            {
                Card card = this.Deck[i];
                if (this.IsDeactivationNecessary(card, allCharacters))
                {
                    Turn.Character.OnCardDeactivated(card);
                    int number = Turn.Number;
                    if (allCharacters)
                    {
                        int index = Party.IndexOf(card.PlayedOwner);
                        if (index >= 0)
                        {
                            Turn.Number = index;
                        }
                    }
                    bool flag = (Turn.State == GameStateType.Penalty) || (Turn.State == GameStateType.Discard);
                    if (!flag)
                    {
                        flag = ((card.ActionDeactivate(true) || (Turn.State == GameStateType.Damage)) || (Turn.State == GameStateType.Ambush)) || (Turn.State == GameStateType.PickHand);
                    }
                    if (!flag)
                    {
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (((window != null) && (window.Popup.Count > 0)) && (card.GetAllowedPowersCount(this.CardAction) > 1))
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        card.ReturnCard(card);
                    }
                    if (allCharacters)
                    {
                        Turn.Number = number;
                        card.Show(false);
                    }
                }
            }
            this.Refresh();
        }
    }

    public virtual void Show(bool isVisible)
    {
        this.Visible = isVisible;
    }

    public void Shuffle(int numCards)
    {
        if (numCards > 1)
        {
            <Shuffle>c__AnonStorey122 storey = new <Shuffle>c__AnonStorey122 {
                window = UI.Window as GuiWindowLocation
            };
            if (storey.window != null)
            {
                storey.window.shufflePanel.transform.position = base.transform.position;
                storey.window.shufflePanel.transform.localScale = this.Size;
                storey.window.shufflePanel.Show(true);
                storey.window.shufflePanel.SortingOrder = 100;
                LeanTween.delayedCall(storey.window.shufflePanel.Shuffle(numCards), new Action(storey.<>m__FB));
            }
        }
    }

    protected virtual void Start()
    {
        if (this.Tray != null)
        {
            this.Tray.Initialize();
            this.Tray.Show(false);
        }
    }

    public virtual bool Validate(Deck deck)
    {
        bool flag = false;
        if (deck != null)
        {
            for (int i = deck.Count - 1; i >= 0; i--)
            {
                if (i < deck.Count)
                {
                    if (deck[i].Displayed && this.Validate(deck[i], ActionType.Display))
                    {
                        flag = true;
                    }
                    else if (this.Validate(deck[i], this.CardAction))
                    {
                        flag = true;
                    }
                }
            }
        }
        if (flag && this.AutoRefresh)
        {
            this.Refresh();
        }
        return (flag && this.AutoRefresh);
    }

    protected virtual bool Validate(Card card, ActionType action)
    {
        int num;
        Card card2;
        string str;
        CardPower playedCardPower = card.GetPlayedCardPower(out num, out card2);
        if (((playedCardPower != null) && playedCardPower.IsPowerDeactivationAllowed(card)) && (!playedCardPower.IsActionValid(action, card) && playedCardPower.IsValidationRequired()))
        {
            int number = Turn.Number;
            int index = Party.IndexOf(card2.PlayedOwner);
            if (index >= 0)
            {
                int num4;
                Card card3;
                Turn.Number = index;
                CardPower power2 = card.GetPlayedCardPower(out num4, out card3);
                card.ActionDeactivate(true);
                if (card.IsActionValid(action) && this.OnGuiDrop(card))
                {
                    card.ActionActivate(action);
                    if (card.PlayedPower < 0)
                    {
                        power2.Activate(card);
                        card.SetPowerInfo(num4, card3);
                        GuiWindowLocation window = UI.Window as GuiWindowLocation;
                        if (window != null)
                        {
                            window.Popup.Show(false);
                        }
                    }
                }
                else
                {
                    card.ReturnCard(card);
                }
                Turn.Number = number;
            }
        }
        CharacterPower playedCharacterPower = card.GetPlayedCharacterPower(out num, out str);
        if ((playedCharacterPower != null) && !playedCharacterPower.IsLegalActivation())
        {
            card.ActionDeactivate(true);
        }
        return ((playedCardPower != null) || (playedCharacterPower != null));
    }

    [DebuggerHidden]
    protected IEnumerator WaitForTime(float time) => 
        new <WaitForTime>c__Iterator45 { 
            time = time,
            <$>time = time,
            <>f__this = this
        };

    public virtual bool AutoRefresh =>
        true;

    public virtual Deck Deck
    {
        get => 
            this.myDeck;
        set
        {
            if (this.myDeck != value)
            {
                this.myDeck = value;
                this.Refresh();
            }
        }
    }

    public virtual Vector3 Scale =>
        new Vector3(this.CardSize, this.CardSize, 1f);

    public virtual Vector3 Size =>
        new Vector3(this.DeckSize, this.DeckSize, 1f);

    public bool Visible { get; private set; }

    [CompilerGenerated]
    private sealed class <Shuffle>c__AnonStorey122
    {
        internal GuiWindowLocation window;

        internal void <>m__FB()
        {
            this.window.shufflePanel.Show(false);
        }
    }

    [CompilerGenerated]
    private sealed class <WaitForTime>c__Iterator45 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal float <$>time;
        internal GuiLayout <>f__this;
        internal float time;

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
                case 1:
                    if (this.time > 0f)
                    {
                        if (this.<>f__this.Visible)
                        {
                            this.time -= Time.deltaTime;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        return true;
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

