using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class GuiPanelStoreTreasureReveal : GuiPanelBackStack
{
    [Tooltip("reference to the bot left card reveal setup in our hierarchy")]
    public CardRevealer BotLeftCard;
    [Tooltip("reference to the bot right card reveal setup in our hierarchy")]
    public CardRevealer BotRightCard;
    [Tooltip("reference to the buy button in this panel")]
    public GuiButton buyButton;
    private List<Animator> cardAnimators;
    [Tooltip("reference to the chest animator in our hierarchy")]
    public Animator ChestAnimator;
    private string currentBotLeftAnim = string.Empty;
    private string currentBotRightAnim = string.Empty;
    private string currentChestAnim = string.Empty;
    private string currentTopLeftAnim = string.Empty;
    private string currentTopRightAnim = string.Empty;
    private List<GuiLabel> deckNumberLabels;
    [Tooltip("reference to the status/message label in this panel")]
    public GuiLabel messageLabel;
    [Tooltip("reference to the open another button string and functionality")]
    public StrRefType OpenAnotherButtonText;
    [Tooltip("reference to the 'open chest' button in this panel")]
    public GuiButton openButton;
    private ButtonFunctionality openButtonFunctionality;
    [Tooltip("reference to the open button string and functionality")]
    public StrRefType OpenButtonText;
    [Tooltip("reference to the reveal all button string and functionality")]
    public StrRefType RevealButtonText;
    [Tooltip("list of cards currently showing in this panel")]
    public List<Card> revealedCards;
    [Tooltip("number of cards/items facing up currently in this panel\t\t\t\t")]
    public int showingCards;
    [Tooltip("reference to the store manager window from this panel")]
    public GuiWindowStore StoreManager;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the top left card reveal setup in our hierarchy")]
    public CardRevealer TopLeftCard;
    [Tooltip("reference to the top right card reveal setup in our hierarchy")]
    public CardRevealer TopRightCard;
    [Tooltip("list of unrevealed cards")]
    public List<Card> unrevealedCards;
    private Card zoomedCard;

    public static void BoosterOpened()
    {
    }

    public Transform CardSizeReference(Transform t)
    {
        if (t.name.ToLower() == "card_back")
        {
            return t;
        }
        for (int i = 0; i < t.childCount; i++)
        {
            Transform transform = this.CardSizeReference(t.GetChild(i));
            if (transform != null)
            {
                return transform;
            }
        }
        return null;
    }

    [DebuggerHidden]
    public static IEnumerator ChestOpened() => 
        new <ChestOpened>c__Iterator72();

    public override void Clear()
    {
        if ((this.revealedCards != null) && (this.revealedCards.Count > 0))
        {
            for (int i = this.revealedCards.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(this.revealedCards[i].gameObject);
            }
        }
        this.revealedCards.Clear();
        this.unrevealedCards.Clear();
    }

    [DebuggerHidden]
    private IEnumerator EndTreasureChest(float f, GuiPanelStoreOverlay.RevealAllTreasureChestCardsCallback callback = null) => 
        new <EndTreasureChest>c__Iterator74 { 
            f = f,
            callback = callback,
            <$>f = f,
            <$>callback = callback,
            <>f__this = this
        };

    private Card GetTopCard(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(Geometry.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity);
        if (hitd == 0)
        {
            return null;
        }
        for (int i = 0; i < hitd.transform.parent.childCount; i++)
        {
            Card component = hitd.transform.parent.GetChild(i).GetComponent<Card>();
            if (component != null)
            {
                return component;
            }
        }
        return hitd.collider.transform.parent.parent.GetComponent<Card>();
    }

    public override void Initialize()
    {
        this.openButtonFunctionality = ButtonFunctionality.Open;
        this.openButton.Text = this.OpenButtonText.ToString();
        this.revealedCards = new List<Card>();
        this.unrevealedCards = new List<Card>();
        this.showingCards = 0;
        this.cardAnimators = new List<Animator>();
        this.cardAnimators.Add(this.TopLeftCard.Animator);
        this.cardAnimators.Add(this.TopRightCard.Animator);
        this.cardAnimators.Add(this.BotRightCard.Animator);
        this.cardAnimators.Add(this.BotLeftCard.Animator);
        this.deckNumberLabels = new List<GuiLabel>();
        this.deckNumberLabels.Add(this.TopLeftCard.DeckNumberLabel);
        this.deckNumberLabels.Add(this.TopRightCard.DeckNumberLabel);
        this.deckNumberLabels.Add(this.BotRightCard.DeckNumberLabel);
        this.deckNumberLabels.Add(this.BotLeftCard.DeckNumberLabel);
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.messageLabel.Text = string.Empty;
        this.Show(false);
    }

    public bool IsCardsOnScreen()
    {
        if ((this.revealedCards.Count <= 0) && (this.unrevealedCards.Count <= 0))
        {
            return false;
        }
        return true;
    }

    private void NormalOpen()
    {
        if (Game.Network.CurrentUser.Chests > 0)
        {
            UI.Busy = true;
            for (int i = this.revealedCards.Count - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(this.revealedCards[i].gameObject);
            }
            this.revealedCards.Clear();
            this.unrevealedCards.Clear();
            Resources.UnloadUnusedAssets();
            this.SetAnim(this.ChestAnimator, ref this.currentChestAnim, "TryOpen");
            AudioClip sfx = Game.UI.GetSfx(SoundEffectType.ChestRumble);
            UI.Sound.Play(sfx);
            Game.Network.OpenTreasureChest(sfx);
            this.StoreManager.Refresh();
            this.buyButton.Disable(true);
            this.openButton.Disable(true);
            this.openButton.Text = this.RevealButtonText.ToString();
            this.openButtonFunctionality = ButtonFunctionality.RevealAll;
        }
        else
        {
            this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Treasure_Buy);
        }
    }

    private void OnBuyButtonPushed()
    {
        if (!UI.Busy)
        {
            this.ResetChest();
            UI.Busy = true;
            this.StoreManager.SwitchTo(GuiWindowStore.StorePanelType.Treasure_Buy);
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (!UI.Busy)
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
                if (topCard != null)
                {
                    if (!topCard.Visible)
                    {
                        base.StartCoroutine(this.ShowCardFrontDelayed(0f, topCard));
                        if (this.revealedCards.Count == 4)
                        {
                            this.openButtonFunctionality = ButtonFunctionality.OpenAnother;
                            this.openButton.Text = this.OpenAnotherButtonText.ToString();
                            this.buyButton.Disable(false);
                            UI.Sound.Play(SoundEffectType.ChestDisappear);
                        }
                    }
                    else if (topCard.Visible && !LeanTween.isTweening(topCard.gameObject))
                    {
                        this.ZoomCard(topCard);
                    }
                }
            }
        }
    }

    private void OnOpenButtonPushed()
    {
        if (!UI.Busy)
        {
            if (this.zoomedCard != null)
            {
                this.UnZoomCard();
            }
            else if (this.openButtonFunctionality == ButtonFunctionality.Open)
            {
                this.NormalOpen();
            }
            else if (this.openButtonFunctionality == ButtonFunctionality.OpenAnother)
            {
                UI.Busy = true;
                base.StartCoroutine(this.ResetChestAnim());
            }
            else
            {
                this.RevealAll(null);
            }
        }
    }

    private void PlayOpenAnimation(Card card)
    {
        if (card.transform.parent == this.TopLeftCard.Animator.transform)
        {
            this.SetAnim(this.TopLeftCard.Animator, ref this.currentTopLeftAnim, "Open");
        }
        else if (card.transform.parent == this.TopRightCard.Animator.transform)
        {
            this.SetAnim(this.TopRightCard.Animator, ref this.currentTopRightAnim, "Open");
        }
        else if (card.transform.parent == this.BotRightCard.Animator.transform)
        {
            this.SetAnim(this.BotRightCard.Animator, ref this.currentBotRightAnim, "Open");
        }
        else if (card.transform.parent == this.BotLeftCard.Animator.transform)
        {
            this.SetAnim(this.BotLeftCard.Animator, ref this.currentBotLeftAnim, "Open");
        }
    }

    public void ProcessRewards(string[] cards)
    {
        this.SaveCardsToVault(cards);
        this.SetAnim(this.ChestAnimator, ref this.currentChestAnim, "Open");
        UI.Sound.Play(SoundEffectType.StoreChestOpen);
        for (int i = 0; i < cards.Length; i++)
        {
            Animator animator = this.cardAnimators[i];
            GuiLabel label = this.deckNumberLabels[i];
            Card item = CardTable.Create(cards[i]);
            if (item != null)
            {
                this.unrevealedCards.Add(item);
                item.transform.SetParent(animator.transform);
                this.ShowRarity(true, item);
                this.ShowDeckNumber(true, item);
                item.Show(false);
                label.Text = "DECK " + item.Set;
            }
        }
    }

    public override void Refresh()
    {
        if (Game.Network.CurrentUser.Chests > 0)
        {
            this.buyButton.Show(false);
        }
        else
        {
            this.buyButton.Show(true);
        }
        AlertManager.SeenAlert(AlertManager.AlertType.SeenStoreTreasureReveal);
        AlertManager.HandleAlerts();
    }

    private void ResetCardAnims()
    {
        if (this.TopLeftCard.Animator.isInitialized)
        {
            this.SetAnim(this.TopLeftCard.Animator, ref this.currentTopLeftAnim, "Reset");
        }
        if (this.BotLeftCard.Animator.isInitialized)
        {
            this.SetAnim(this.BotLeftCard.Animator, ref this.currentBotLeftAnim, "Reset");
        }
        if (this.TopRightCard.Animator.isInitialized)
        {
            this.SetAnim(this.TopRightCard.Animator, ref this.currentTopRightAnim, "Reset");
        }
        if (this.BotRightCard.Animator.isInitialized)
        {
            this.SetAnim(this.BotRightCard.Animator, ref this.currentBotRightAnim, "Reset");
        }
    }

    public void ResetChest()
    {
        this.ResetCardAnims();
        if (this.ChestAnimator.isInitialized)
        {
            this.SetAnim(this.ChestAnimator, ref this.currentChestAnim, "Reset");
        }
    }

    [DebuggerHidden]
    private IEnumerator ResetChestAnim() => 
        new <ResetChestAnim>c__Iterator73 { <>f__this = this };

    public void RevealAll(GuiPanelStoreOverlay.RevealAllTreasureChestCardsCallback callback = null)
    {
        int num = 0;
        for (int i = this.unrevealedCards.Count - 1; i >= 0; i--)
        {
            base.StartCoroutine(this.ShowCardFrontDelayed(0.25f * num++, this.unrevealedCards[i]));
        }
        this.openButton.Disable(true);
        this.openButton.Text = this.OpenAnotherButtonText.ToString();
        this.openButtonFunctionality = ButtonFunctionality.OpenAnother;
        this.showingCards = 0;
        base.StartCoroutine(this.EndTreasureChest(0.25f * num, callback));
    }

    private void SaveCardsToVault(string[] cards)
    {
        Collection.Push(cards);
    }

    private void SetAnim(Animator anim, ref string currentAnim, string nextAnim)
    {
        if (currentAnim != nextAnim)
        {
            if (currentAnim != string.Empty)
            {
                anim.ResetTrigger(currentAnim);
            }
            anim.SetTrigger(nextAnim);
            currentAnim = nextAnim;
        }
    }

    public static void SetMessageText(string str)
    {
        GuiWindow current = GuiWindow.Current;
        if (current is GuiWindowStore)
        {
            GuiWindowStore store = current as GuiWindowStore;
            store.treasureRevealPanel.messageLabel.Text = str;
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        if (isVisible)
        {
            this.openButtonFunctionality = ButtonFunctionality.Open;
            this.Refresh();
        }
        else
        {
            this.Clear();
            this.openButton.Disable(false);
            UI.Busy = false;
        }
    }

    private void ShowCardFront(Card card)
    {
        Transform transform = this.CardSizeReference(card.transform.parent);
        card.transform.localScale = transform.localScale;
        card.transform.position = transform.position;
        card.Show(CardSideType.Front);
        UI.Sound.Play(SoundEffectType.RewardChestCardExplosion);
        this.PlayOpenAnimation(card);
        this.revealedCards.Add(card);
        this.unrevealedCards.Remove(card);
    }

    [DebuggerHidden]
    private IEnumerator ShowCardFrontDelayed(float f, Card card) => 
        new <ShowCardFrontDelayed>c__Iterator75 { 
            f = f,
            card = card,
            <$>f = f,
            <$>card = card,
            <>f__this = this
        };

    private void ShowDeckNumber(bool isVisible, Card card)
    {
        CardRevealer topLeftCard = null;
        if (card.transform.parent == this.TopLeftCard.Animator.transform)
        {
            topLeftCard = this.TopLeftCard;
        }
        else if (card.transform.parent == this.TopRightCard.Animator.transform)
        {
            topLeftCard = this.TopRightCard;
        }
        else if (card.transform.parent == this.BotRightCard.Animator.transform)
        {
            topLeftCard = this.BotRightCard;
        }
        else if (card.transform.parent == this.BotLeftCard.Animator.transform)
        {
            topLeftCard = this.BotLeftCard;
        }
        if (topLeftCard != null)
        {
            for (int i = 0; i < topLeftCard.DeckNumberLabel.transform.parent.childCount; i++)
            {
                topLeftCard.DeckNumberLabel.transform.parent.GetChild(i).gameObject.SetActive(isVisible);
            }
        }
    }

    private void ShowRarity(bool isVisible, Card card)
    {
        CardRevealer topLeftCard = null;
        if (card.transform.parent == this.TopLeftCard.Animator.transform)
        {
            topLeftCard = this.TopLeftCard;
        }
        else if (card.transform.parent == this.TopRightCard.Animator.transform)
        {
            topLeftCard = this.TopRightCard;
        }
        else if (card.transform.parent == this.BotRightCard.Animator.transform)
        {
            topLeftCard = this.BotRightCard;
        }
        else if (card.transform.parent == this.BotLeftCard.Animator.transform)
        {
            topLeftCard = this.BotLeftCard;
        }
        if (topLeftCard != null)
        {
            topLeftCard.UncommonRarity.SetActive(isVisible && (card.Rarity == RarityType.Uncommon));
            topLeftCard.RareRarity.SetActive(isVisible && (card.Rarity == RarityType.Rare));
            topLeftCard.EpicRarity.SetActive(isVisible && (card.Rarity == RarityType.Epic));
            topLeftCard.LegendaryRarity.SetActive(isVisible && (card.Rarity == RarityType.Legendary));
        }
    }

    private void UnZoomCard()
    {
        if (this.zoomedCard != null)
        {
            UI.Busy = true;
            LeanTween.delayedCall(this.zoomedCard.OnGuiZoom(false), new Action(this.UnZoomCardDone));
        }
    }

    private void UnZoomCardDone()
    {
        this.ShowDeckNumber(true, this.zoomedCard);
        this.ShowRarity(true, this.zoomedCard);
        UI.Busy = false;
        UI.Zoomed = false;
        this.zoomedCard = null;
    }

    private void ZoomCard(Card card)
    {
        if (card != null)
        {
            this.ShowRarity(false, card);
            this.ShowDeckNumber(false, card);
            UI.Busy = true;
            UI.Zoomed = true;
            float duration = card.OnGuiZoom(true);
            this.zoomedCard = card;
            UI.Lock(duration);
        }
    }

    [CompilerGenerated]
    private sealed class <ChestOpened>c__Iterator72 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiWindowStore <store>__1;
        internal GuiWindow <window>__0;

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
                    this.<window>__0 = GuiWindow.Current;
                    if (this.<window>__0 == null)
                    {
                        break;
                    }
                    this.<store>__1 = this.<window>__0 as GuiWindowStore;
                    if (this.<store>__1 == null)
                    {
                        break;
                    }
                    this.<store>__1.treasureRevealPanel.TopLeftCard.Animator.SetTrigger("Start");
                    UI.Sound.Play(SoundEffectType.ChestCardMoveToQuadrant2);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 1;
                    goto Label_01AC;

                case 1:
                    this.<store>__1.treasureRevealPanel.TopRightCard.Animator.SetTrigger("Start");
                    UI.Sound.Play(SoundEffectType.ChestCardMoveToQuadrant1);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 2;
                    goto Label_01AC;

                case 2:
                    this.<store>__1.treasureRevealPanel.BotLeftCard.Animator.SetTrigger("Start");
                    UI.Sound.Play(SoundEffectType.ChestCardMoveToQuadrant3);
                    this.$current = new WaitForSeconds(0.25f);
                    this.$PC = 3;
                    goto Label_01AC;

                case 3:
                    this.<store>__1.treasureRevealPanel.BotRightCard.Animator.SetTrigger("Start");
                    UI.Sound.Play(SoundEffectType.ChestCardMoveToQuadrant4);
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 4;
                    goto Label_01AC;

                case 4:
                    this.<store>__1.treasureRevealPanel.openButton.Disable(false);
                    UI.Busy = false;
                    break;

                default:
                    goto Label_01AA;
            }
            this.$PC = -1;
        Label_01AA:
            return false;
        Label_01AC:
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
    private sealed class <EndTreasureChest>c__Iterator74 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelStoreOverlay.RevealAllTreasureChestCardsCallback <$>callback;
        internal float <$>f;
        internal GuiPanelStoreTreasureReveal <>f__this;
        internal int <i>__0;
        internal GuiPanelStoreOverlay.RevealAllTreasureChestCardsCallback callback;
        internal float f;

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
                    this.$current = new WaitForSeconds(2f * this.f);
                    this.$PC = 1;
                    goto Label_0159;

                case 1:
                    UI.Sound.Play(SoundEffectType.ChestDisappear);
                    this.<>f__this.buyButton.Disable(false);
                    this.<>f__this.openButton.Disable(false);
                    if (this.callback == null)
                    {
                        break;
                    }
                    this.<>f__this.SetAnim(this.<>f__this.ChestAnimator, ref this.<>f__this.currentChestAnim, "End");
                    this.<>f__this.ResetCardAnims();
                    this.<i>__0 = this.<>f__this.revealedCards.Count - 1;
                    while (this.<i>__0 >= 0)
                    {
                        UnityEngine.Object.Destroy(this.<>f__this.revealedCards[this.<i>__0].gameObject);
                        this.<i>__0--;
                    }
                    this.<>f__this.revealedCards.Clear();
                    this.<>f__this.unrevealedCards.Clear();
                    this.$current = new WaitForSeconds(2f);
                    this.$PC = 2;
                    goto Label_0159;

                case 2:
                    this.callback();
                    break;

                default:
                    goto Label_0157;
            }
            this.$PC = -1;
        Label_0157:
            return false;
        Label_0159:
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
    private sealed class <ResetChestAnim>c__Iterator73 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal GuiPanelStoreTreasureReveal <>f__this;
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
                    this.<>f__this.SetAnim(this.<>f__this.ChestAnimator, ref this.<>f__this.currentChestAnim, "End");
                    this.<>f__this.ResetCardAnims();
                    this.<i>__0 = this.<>f__this.revealedCards.Count - 1;
                    while (this.<i>__0 >= 0)
                    {
                        UnityEngine.Object.Destroy(this.<>f__this.revealedCards[this.<i>__0].gameObject);
                        this.<i>__0--;
                    }
                    this.<>f__this.revealedCards.Clear();
                    this.<>f__this.unrevealedCards.Clear();
                    this.$current = new WaitForSeconds(0.5f);
                    this.$PC = 1;
                    goto Label_0152;

                case 1:
                    this.<>f__this.SetAnim(this.<>f__this.ChestAnimator, ref this.<>f__this.currentChestAnim, "Reset");
                    UI.Sound.Play(SoundEffectType.ChestAppear);
                    this.$current = new WaitForSeconds(1f);
                    this.$PC = 2;
                    goto Label_0152;

                case 2:
                    this.<>f__this.openButtonFunctionality = GuiPanelStoreTreasureReveal.ButtonFunctionality.Open;
                    UI.Busy = false;
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_0152:
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
    private sealed class <ShowCardFrontDelayed>c__Iterator75 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal float <$>f;
        internal GuiPanelStoreTreasureReveal <>f__this;
        internal Card card;
        internal float f;

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
                    if (this.f == 0f)
                    {
                        this.<>f__this.ShowCardFront(this.card);
                    }
                    this.$current = new WaitForSeconds(this.f);
                    this.$PC = 1;
                    return true;

                case 1:
                    if (this.f != 0f)
                    {
                        this.<>f__this.ShowCardFront(this.card);
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

    public enum ButtonFunctionality
    {
        Open,
        OpenAnother,
        RevealAll
    }

    [Serializable]
    public class CardRevealer
    {
        [Tooltip("reference to the card animator in our hierarchy")]
        public UnityEngine.Animator Animator;
        [Tooltip("reference to the deck number label in our hierarchy")]
        public GuiLabel DeckNumberLabel;
        [Tooltip("reference to the epic vfx game object in our hierarchy")]
        public GameObject EpicRarity;
        [Tooltip("reference to the legendary vfx game object in our hierarchy")]
        public GameObject LegendaryRarity;
        [Tooltip("reference to the rare vfx game object in our hierarchy")]
        public GameObject RareRarity;
        [Tooltip("reference to the uncommon vfx game object in our hierarchy")]
        public GameObject UncommonRarity;
    }
}

