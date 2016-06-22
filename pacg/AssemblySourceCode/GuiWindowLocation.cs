using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowLocation : GuiWindow
{
    [Tooltip("reference to the blessings panel in our hierarchy")]
    public GuiPanelBlessings blessingsPanel;
    [Tooltip("reference to the box panel in this scene")]
    public GuiPanelBox boxPanel;
    private static readonly Color buttonAvailableColor = new Color(1f, 1f, 1f);
    private static readonly Color buttonUnavailableColor = new Color(0.588f, 0.443f, 0.142f);
    [Tooltip("reference to the cancel button in our hierarchy")]
    public GuiButton cancelButton;
    [Tooltip("reference to the character sheet panel in this scene")]
    public GuiPanelCharacter characterPanel;
    [Tooltip("reference to the \"show character sheet\" button in our hierarchy")]
    public GuiButtonRegion characterSheetButton;
    [Tooltip("reference to the choose card type panel in our hierarchy")]
    public GuiPanelChooseCardType chooseTypePanel;
    [Tooltip("reference to the \"close location\" panel in our hierarchy")]
    public GuiPanelLocationClose closeLocationPanel;
    [Tooltip("reference to the combat intro vfx in our hierarchy")]
    public Animator combatIntroAnimator;
    [Tooltip("reference to the turn commands panel in our hierarchy")]
    public GuiPanelCommands commandsPanel;
    [Tooltip("reference to the \"confirm examine\" button in our hierarchy")]
    public GuiButton confirmButton;
    [Tooltip("reference to the death popup panel in our hierarchy")]
    public GuiPanelDeath deathPanel;
    [Tooltip("reference to the dice panel in our hierarchy")]
    public GuiPanelDice dicePanel;
    private TKDiceRecognizer diceRecognizer;
    private Card draggedCard;
    private GuiLayout draggedLayout;
    private Vector2 dragLocation;
    private TKPanRecognizer dragRecognizer;
    [Tooltip("reference to the \"ongoing effects\" panel in our hierarchy")]
    public GuiPanelEffects effectsPanel;
    [Tooltip("reference to the \"encounter this card\" button in our hierarchy")]
    public GuiButton encounterButton;
    [Tooltip("reference to the \"examine location deck\" button in our hierarchy")]
    public GuiButton examineButton;
    [Tooltip("reference to the heal panel in our hierarchy")]
    public GuiPanelHeal healPanel;
    [Tooltip("reference to the ui help panel in our hierarchy")]
    public GuiPanelTutorialHelp helpPanel;
    private bool isMapShowing;
    [Tooltip("reference to the banish pile in our hierarchy")]
    public GuiLayoutStack layoutBanish;
    [Tooltip("reference to the blessings deck layout in our hierarchy")]
    public GuiLayoutHidden layoutBlessings;
    [Tooltip("reference to the bury pile in our hierarchy")]
    public GuiLayoutStack layoutBury;
    [Tooltip("reference to the discard pile in our hierarchy")]
    public GuiLayoutStack layoutDiscard;
    [Tooltip("reference to the \"examine tray\" in our hierarchy")]
    public GuiLayoutExamine layoutExamine;
    [Tooltip("reference to the explore button in our hierarchy")]
    public GuiLayoutExplore layoutExplore;
    [Tooltip("reference to the hand in our hierarchy")]
    public GuiLayoutHand layoutHand;
    [Tooltip("reference to the location deck in our hierarchy")]
    public GuiLayoutLocation layoutLocation;
    [Tooltip("reference to the recharge pile in our hierarchy")]
    public GuiLayoutStack layoutRecharge;
    [Tooltip("reference to the reveal area in our hierarchy")]
    public GuiLayoutReveal layoutReveal;
    [Tooltip("reference to the \"share cards\" layout in our hierarchy")]
    public GuiLayoutShare layoutShare;
    [Tooltip("reference to the \"summoner card\" display in our hierarchy")]
    public GuiLayoutCard layoutSummoner;
    [Tooltip("reference to the \"pick tray\" in our hierarchy")]
    public GuiLayoutTray layoutTray;
    [Tooltip("reference to the location closed overlay in this scene")]
    public GameObject locationClosedOverlay;
    [Tooltip("reference to the location panel in our hierarchy")]
    public GuiPanelLocation locationPanel;
    [Tooltip("reference to the map button in our hierarchy")]
    public GuiButton mapButton;
    [Tooltip("reference to the map in our hierarchy")]
    public GuiPanelMap mapPanel;
    [Tooltip("reference to the max hand size label in this scene")]
    public GuiLabel maxHandSizeLabel;
    [Tooltip("reference to the popup message panel in our hierarchy")]
    public GuiPanelMessage messagePanel;
    [Tooltip("reference to the party panel in our hierarchy")]
    public GuiPanelParty partyPanel;
    [Tooltip("reference to the game phase panel in our hierarchy")]
    public GuiPanelPhases phasesPanel;
    [Tooltip("reference to the \"may\" menu panel in our hierarchy")]
    public GuiPanelMenuPopup popupMenu;
    [Tooltip("reference to the powers panel in our hierarchy")]
    public GuiPanelPowers powersPanel;
    private GuiLayout prevHoveredLayout;
    [Tooltip("reference to the proceed button in our hierarchy")]
    public GuiButton proceedButton;
    [Tooltip("reference to the \"screen shade\" panel in this scene")]
    public GuiPanelShade shadePanel;
    [Tooltip("reference to the share button in our hierarchy")]
    public GuiButton shareButton;
    [Tooltip("reference to the share button in our hierarchy")]
    public Animator shareButtonAnimator;
    [Tooltip("reference to the share cards panel in our hierarchy")]
    public GuiPanelShare sharePanel;
    [Tooltip("reference to the \"card shuffle\" animation in our hierarchy")]
    public GuiPanelShuffler shufflePanel;
    [Tooltip("reference to the skill selection panel in our hierarchy")]
    public GuiPanelSkills skillsPanel;
    private bool startInMap;
    [Tooltip("reference to the status panel in our hierarchy")]
    public GuiPanelStatus statusPanel;
    [Tooltip("reference to the \"summon\" button in our hierarchy")]
    public GuiButton summonButton;
    private TKTapRecognizer tapRecognizer;
    [Tooltip("reference to the target panel in this scene")]
    public GuiPanelTarget targetPanel;
    [Tooltip("reference to the \"temp location close\" panel in this scene")]
    public GuiPanelTempClose tempClosePanel;
    [Tooltip("reference to the \"villain fanfare\" panel in this scene")]
    public GuiPanelVillain villainPanel;
    private bool zoomedDiceHidden;
    [Tooltip("reference to the zoom panel in our hierarchy")]
    public GuiPanelZoomMenu zoomPanel;

    protected override void Awake()
    {
        base.Awake();
        Location.Load(Location.Destination);
        this.startInMap = Location.StartInMap;
        Location.StartInMap = false;
    }

    public void Bury(Card card)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.35f, card, Turn.Character.Bury, this.layoutBury, DeckPositionType.Top));
    }

    public void CancelAllPowers(bool allCharacters, bool includeAutomatic)
    {
        Turn.Canceling = true;
        this.Popup.Show(false);
        Turn.CancelAllPowers(allCharacters, includeAutomatic);
        this.layoutDiscard.ReturnCards(allCharacters);
        this.layoutRecharge.ReturnCards(allCharacters);
        this.layoutBury.ReturnCards(allCharacters);
        this.layoutBanish.ReturnCards(allCharacters);
        this.layoutReveal.ReturnCards(allCharacters);
        if (allCharacters)
        {
            int number = Turn.Number;
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                for (int j = 0; j < Party.Characters[i].Hand.Count; j++)
                {
                    if (Party.Characters[i].Hand[j].Revealed)
                    {
                        Turn.Number = i;
                        Party.Characters[i].Hand[j].ActionDeactivate(true);
                    }
                }
            }
            Turn.Number = number;
        }
        this.layoutHand.Refresh();
        if (!this.dicePanel.IsDiceVisible())
        {
            this.dicePanel.Fade(true, 0.15f);
        }
        if ((Turn.Checks != null) && (Turn.Check != SkillCheckType.None))
        {
            SkillCheckType skill = Turn.Owner.GetBestSkillCheck(Turn.Checks).skill;
            this.dicePanel.SetCheck(Turn.Card, Turn.Checks, skill);
        }
        Turn.Refresh();
        Turn.Canceling = false;
    }

    private void CancelButtonPushed()
    {
        UI.Busy = false;
        this.CancelAllPowers(false, false);
        Turn.Cancel();
        this.RefreshCancelButton();
        this.GlowLayouts(false, null);
    }

    public override void Close()
    {
        base.Close();
        this.powersPanel.enabled = false;
        this.layoutHand.enabled = false;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Recharge.Count; j++)
            {
                Party.Characters[i].Recharge[j].Show(false);
            }
        }
    }

    private void CloseSubWindows()
    {
        this.UnZoomCard();
        if (this.layoutExamine.Visible)
        {
            bool animations = this.layoutExamine.Animations;
            this.layoutExamine.Animations = false;
            this.layoutExamine.Show(false);
            this.layoutExamine.Animations = animations;
        }
        this.effectsPanel.Show(false);
    }

    public void Discard(Card card)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.7f, card, Turn.Character.Discard, this.layoutDiscard, DeckPositionType.Top));
    }

    public void DiscardToLayout(Card card)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.7f, card, this.layoutDiscard.Deck, this.layoutDiscard, DeckPositionType.Top));
    }

    public void Draw(Card card)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.35f, card, Turn.Character.Hand, this.layoutHand, DeckPositionType.None));
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            Party.Characters[i].LockInDisplayed(true);
        }
        this.RefreshCancelButton();
    }

    public void DrawCardsFromBox(Card[] cards, Deck deck, int turn)
    {
        Game.Instance.StartCoroutine(this.DrawCardsFromBoxToDeckCoroutine(cards, deck, turn));
    }

    [DebuggerHidden]
    private IEnumerator DrawCardsFromBoxToDeckCoroutine(Card[] cards, Deck deck, int turn) => 
        new <DrawCardsFromBoxToDeckCoroutine>c__Iterator89 { 
            turn = turn,
            cards = cards,
            deck = deck,
            <$>turn = turn,
            <$>cards = cards,
            <$>deck = deck,
            <>f__this = this
        };

    public void DrawFromBox(Card card)
    {
        Game.Instance.StartCoroutine(this.DrawFromBoxCoroutine(card));
    }

    [DebuggerHidden]
    private IEnumerator DrawFromBoxCoroutine(Card card) => 
        new <DrawFromBoxCoroutine>c__Iterator8A { 
            card = card,
            <$>card = card,
            <>f__this = this
        };

    public void DropCard()
    {
        if (this.draggedCard != null)
        {
            this.DropCardOnLayout(this.draggedCard, this.GetCardLayoutManager(this.draggedCard));
            this.draggedCard = null;
            this.draggedLayout = null;
        }
    }

    public bool DropCardOnLayout(Card card, GuiLayout layout)
    {
        bool flag = false;
        bool flag2 = false;
        if (layout.IsDropRestricted(card))
        {
            EffectCardRestrictionPending effect = Turn.Character.GetEffect(EffectType.CardRestrictionPending) as EffectCardRestrictionPending;
            if ((effect != null) && effect.Match(card))
            {
                effect.Invoke(card, layout.CardAction);
            }
            flag2 = true;
        }
        if (!flag2)
        {
            GuiLayout cardLayoutManager = this.GetCardLayoutManager(card);
            flag = layout.OnGuiDrop(card);
            if (flag)
            {
                layout.OnGuiHover(false);
                if (cardLayoutManager != null)
                {
                    if (cardLayoutManager.IsDeactivateOnDrop(card, layout))
                    {
                        card.ActionDeactivate(true);
                    }
                    if (cardLayoutManager.AutoRefresh)
                    {
                        cardLayoutManager.Refresh();
                    }
                    cardLayoutManager.OnGuiHover(false);
                }
                card.ActionActivate(layout.GetActionType(card));
                AnalyticsManager.OnCardAction(card, layout.GetActionType(card));
                this.dicePanel.Refresh();
                Turn.Refresh();
                Party.AutoActivateAbilities();
                this.RefreshCancelButton();
            }
        }
        if ((card.Deck != null) && (card.Deck.Layout != null))
        {
            card.Deck.Layout.OnGuiHover(false);
        }
        return flag;
    }

    private GuiLayout GetCardLayoutManager(Card card)
    {
        if (!card.Revealed && !card.Displayed)
        {
            return card.Deck.Layout;
        }
        return this.layoutReveal;
    }

    public Card GetDraggedCard() => 
        this.draggedCard;

    public GuiLayout GetLayoutDeck(ActionType action)
    {
        if (action == ActionType.Banish)
        {
            return this.layoutBanish;
        }
        if (action == ActionType.Bury)
        {
            return this.layoutBury;
        }
        if (action == ActionType.Discard)
        {
            return this.layoutDiscard;
        }
        if (action == ActionType.Recharge)
        {
            return this.layoutRecharge;
        }
        if (action == ActionType.Top)
        {
            return this.layoutRecharge;
        }
        if (action == ActionType.Reveal)
        {
            return this.layoutReveal;
        }
        return null;
    }

    public GuiLayout GetLayoutDeck(DeckType deck)
    {
        if (deck == DeckType.Bury)
        {
            return this.layoutBury;
        }
        if (deck == DeckType.Character)
        {
            return this.layoutRecharge;
        }
        if (deck == DeckType.Discard)
        {
            return this.layoutDiscard;
        }
        if (deck == DeckType.Hand)
        {
            return this.layoutHand;
        }
        if (deck == DeckType.Revealed)
        {
            return this.layoutReveal;
        }
        if (deck == DeckType.Banish)
        {
            return this.layoutBanish;
        }
        if (deck == DeckType.Location)
        {
            return this.layoutExplore;
        }
        return null;
    }

    public int GetNumCardsInLayout(ActionType deckType)
    {
        if (deckType == ActionType.Discard)
        {
            return this.layoutDiscard.Deck.Count;
        }
        if (deckType == ActionType.Recharge)
        {
            return this.layoutRecharge.Deck.Count;
        }
        if (deckType == ActionType.Top)
        {
            return this.layoutRecharge.Deck.Count;
        }
        if (deckType == ActionType.Banish)
        {
            return this.layoutBanish.Deck.Count;
        }
        if (deckType == ActionType.Bury)
        {
            return this.layoutBury.Deck.Count;
        }
        if (deckType == ActionType.Reveal)
        {
            int num = 0;
            for (int j = 0; j < this.layoutHand.Deck.Count; j++)
            {
                if (this.layoutHand.Deck[j].Revealed)
                {
                    num++;
                }
            }
            return num;
        }
        if (deckType != ActionType.Display)
        {
            return 0;
        }
        int num3 = 0;
        for (int i = 0; i < this.layoutHand.Deck.Count; i++)
        {
            if (this.layoutHand.Deck[i].Displayed)
            {
                num3++;
            }
        }
        return num3;
    }

    private GuiLayout GetTopLayout(Vector2 touchPos)
    {
        RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
        if (hitd.collider != null)
        {
            GuiLayoutStack component = hitd.collider.GetComponent<GuiLayoutStack>();
            if (component != null)
            {
                return component;
            }
        }
        return null;
    }

    public void GlowLayoutDeck(ActionType deckType, bool isGlowing)
    {
        if (deckType == ActionType.Discard)
        {
            this.layoutDiscard.Glow(isGlowing);
        }
        if (deckType == ActionType.Recharge)
        {
            this.layoutRecharge.Glow(isGlowing);
        }
        if (deckType == ActionType.Top)
        {
            this.layoutRecharge.Glow(isGlowing);
        }
        if (deckType == ActionType.Banish)
        {
            this.layoutBanish.Glow(isGlowing);
        }
        if (deckType == ActionType.Bury)
        {
            this.layoutBury.Glow(isGlowing);
        }
        if (deckType == ActionType.Reveal)
        {
            this.layoutReveal.Glow(isGlowing);
        }
        if (deckType == ActionType.Display)
        {
            this.layoutReveal.Glow(isGlowing);
        }
        if (deckType == ActionType.Share)
        {
            this.layoutShare.Glow(isGlowing);
        }
    }

    public void GlowLayouts(bool isVisible, Card card)
    {
        if (isVisible)
        {
            if (Turn.IsActionAllowed(ActionType.Recharge, card))
            {
                this.layoutRecharge.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Top, card))
            {
                this.layoutRecharge.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Discard, card))
            {
                this.layoutDiscard.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Banish, card))
            {
                this.layoutBanish.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Bury, card))
            {
                this.layoutBury.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Reveal, card))
            {
                this.layoutReveal.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Display, card))
            {
                this.layoutReveal.Glow(true);
            }
            if (Turn.IsActionAllowed(ActionType.Share, card))
            {
                this.layoutShare.Glow(true);
            }
        }
        else
        {
            this.layoutRecharge.Glow(false);
            this.layoutDiscard.Glow(false);
            this.layoutBanish.Glow(false);
            this.layoutBury.Glow(false);
            this.layoutReveal.Glow(false);
            this.layoutShare.Glow(false);
        }
    }

    public void Heal(Character character, Card[] cards, DeckPositionType position)
    {
        this.healPanel.Heal(character, cards, position);
    }

    private void HideZoomMenu()
    {
        this.GlowLayouts(false, null);
        if (this.zoomedDiceHidden)
        {
            this.dicePanel.Fade(true, 0.25f);
        }
        this.zoomedDiceHidden = false;
        this.zoomPanel.Show(false);
    }

    public bool IsDragging(Card card) => 
        ((this.draggedCard != null) && this.draggedCard.Equals(card));

    [DebuggerHidden]
    private IEnumerator MoveCardCoroutine(float delay, Card card, Deck deck, GuiLayout layout, DeckPositionType position) => 
        new <MoveCardCoroutine>c__Iterator8B { 
            card = card,
            deck = deck,
            position = position,
            layout = layout,
            delay = delay,
            <$>card = card,
            <$>deck = deck,
            <$>position = position,
            <$>layout = layout,
            <$>delay = delay,
            <>f__this = this
        };

    private void OnCancelButtonPushed()
    {
        if ((!UI.Busy && Turn.IsCancelAllowed()) && !this.layoutExamine.Visible)
        {
            UI.Busy = true;
            this.CloseSubWindows();
            LeanTween.scale(this.cancelButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.CancelButtonPushed));
        }
    }

    private void OnCharacterSheetButtonPushed()
    {
        if (!base.Paused)
        {
            this.CloseSubWindows();
            this.Pause(true);
            this.characterPanel.Show(true);
        }
    }

    private void OnConfirmButtonPushed()
    {
        if (!base.Paused)
        {
            UI.Sound.Play(SoundEffectType.ConfirmProceed);
            Turn.Proceed();
        }
    }

    private void OnEncounterButtonPushed()
    {
        if (!base.Paused)
        {
            Turn.EvadeDeclined = true;
            Turn.Proceed();
        }
    }

    private void OnExamineButtonPushed()
    {
        if (!base.Paused && !GuiPanelExamine.Open)
        {
            this.CloseSubWindows();
            this.layoutExamine.Source = DeckType.Location;
            this.layoutExamine.ModifyTop = false;
            this.layoutExamine.ModifyBottom = false;
            this.layoutExamine.Scroll = false;
            this.layoutExamine.Show(true);
            this.layoutExamine.Refresh();
        }
    }

    private void OnGuiDrag(Vector2 touchLoc)
    {
        if (this.draggedCard != null)
        {
            Vector2 origin = base.ScreenToWorldPoint(touchLoc);
            this.draggedCard.transform.position = (Vector3) origin;
            this.UnZoomCard();
            RaycastHit2D[] hitdArray = Physics2D.RaycastAll(origin, Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
            if (hitdArray != null)
            {
                for (int i = 0; i < hitdArray.Length; i++)
                {
                    GuiLayout component = hitdArray[i].collider.transform.GetComponent<GuiLayout>();
                    if ((component != null) && component.OnGuiHover(this.draggedCard))
                    {
                        if (this.draggedLayout != null)
                        {
                            this.draggedLayout.OnGuiHover(false);
                        }
                        if ((this.prevHoveredLayout != null) && (this.prevHoveredLayout != component))
                        {
                            this.prevHoveredLayout.OnGuiHover(false);
                        }
                        this.prevHoveredLayout = component;
                        return;
                    }
                }
            }
        }
        if (this.prevHoveredLayout != null)
        {
            this.prevHoveredLayout.OnGuiHover(false);
            if (this.draggedLayout != null)
            {
                this.draggedLayout.OnGuiHover(true);
            }
        }
        this.prevHoveredLayout = null;
    }

    private void OnGuiDragEnd(Vector2 touchPos)
    {
        this.GlowLayouts(false, null);
        bool isValid = false;
        if (this.draggedCard != null)
        {
            RaycastHit2D[] hitdArray = Physics2D.RaycastAll(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
            if (hitdArray != null)
            {
                for (int i = 0; i < hitdArray.Length; i++)
                {
                    GuiLayout component = hitdArray[i].collider.transform.GetComponent<GuiLayout>();
                    if (component != null)
                    {
                        isValid = this.DropCardOnLayout(this.draggedCard, component);
                        if (isValid)
                        {
                            break;
                        }
                    }
                }
            }
            if (!isValid)
            {
                isValid = this.DropCardOnLayout(this.draggedCard, this.draggedLayout);
            }
            this.draggedCard.OnGuiDrop(isValid);
            if (!isValid)
            {
                if (this.zoomPanel.Card != null)
                {
                    this.ShowZoomMenu();
                }
            }
            else
            {
                this.zoomPanel.Card = null;
            }
            if (Turn.State != GameStateType.Sacrifice)
            {
                Turn.Refresh();
            }
            if (this.draggedLayout != null)
            {
                this.draggedLayout.OnGuiHover(false);
            }
            if ((this.draggedCard.Deck != null) && (this.draggedCard.Deck.Layout != null))
            {
                this.draggedCard.Deck.Layout.Refresh();
            }
            this.draggedCard = null;
            this.draggedLayout = null;
        }
        if (this.prevHoveredLayout != null)
        {
            this.prevHoveredLayout.OnGuiHover(false);
        }
        this.prevHoveredLayout = null;
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        this.UnZoomCard();
        Card topCard = GuiLayout.GetTopCard(touchPos);
        if (((topCard != null) && (topCard.Deck != null)) && ((topCard.Deck.Layout != null) && topCard.Deck.Layout.OnGuiDrag(topCard)))
        {
            this.draggedCard = topCard;
            this.draggedLayout = this.GetCardLayoutManager(topCard);
            if (topCard == this.zoomPanel.Card)
            {
                this.HideZoomMenu();
            }
            topCard.Deck.Layout.OnGuiHover(true);
            if ((this.draggedLayout == null) || this.draggedLayout.IsDeactivateOnDrag(topCard))
            {
                topCard.ActionDeactivate(true);
            }
            this.GlowLayouts(true, topCard);
            topCard.OnGuiDrag();
        }
    }

    private void OnGuiSwipe(Vector2 direction)
    {
        if (this.dicePanel.Ready && (((Game.GameType != GameType.LocalMultiPlayer) || (Turn.SwitchType == SwitchType.None)) || Rules.IsTurnOwner()))
        {
            if ((Game.GameType == GameType.LocalSinglePlayer) && !Rules.IsTurnOwner())
            {
                Turn.Number = Turn.Current;
                this.Refresh();
            }
            this.dicePanel.Roll(direction);
            this.ShowCancelButton(false);
        }
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.zoomPanel.Card != null)
        {
            UI.Lock(this.UnZoomCard());
        }
        else
        {
            this.OnGuiDragEnd(this.dragLocation);
            Card topCard = GuiLayout.GetTopCard(touchPos);
            if (topCard != null)
            {
                if (topCard.Side == CardSideType.Front)
                {
                    if (!LeanTween.isTweening(topCard.gameObject))
                    {
                        this.ZoomCard(topCard);
                    }
                }
                else if (((topCard.Deck == Location.Current.Deck) && Rules.IsExplorePossible()) && Turn.Explore)
                {
                    UI.Sound.Play(SoundEffectType.GenericFlickCard);
                    this.layoutLocation.OnGuiDrop(topCard);
                }
            }
            else if ((Turn.State != GameStateType.Share) && !this.mapPanel.IsTouchHandled(touchPos))
            {
                GuiLayout topLayout = this.GetTopLayout(touchPos);
                if ((topLayout != null) && (topLayout.Tray != null))
                {
                    this.UnZoomCard();
                    topLayout.Tray.Layout = topLayout;
                    topLayout.Tray.TitleText = topLayout.CardAction.ToText();
                    if (topLayout.CardAction == ActionType.Recharge)
                    {
                        topLayout.Tray.Deck = Turn.Character.Deck;
                        topLayout.Tray.TitleText = StringTableManager.GetUIText(0x21a);
                    }
                    if (topLayout.CardAction == ActionType.Discard)
                    {
                        topLayout.Tray.Deck = Turn.Character.Discard;
                    }
                    if (topLayout.CardAction == ActionType.Bury)
                    {
                        topLayout.Tray.Deck = Turn.Character.Bury;
                    }
                    topLayout.Tray.CardAction = topLayout.CardAction;
                    topLayout.Tray.Show(true);
                }
            }
        }
    }

    public override void OnLoadData()
    {
        this.layoutDiscard.OnLoadData();
        this.layoutRecharge.OnLoadData();
        this.layoutBury.OnLoadData();
        this.layoutBanish.OnLoadData();
        this.layoutSummoner.OnLoadData();
        this.layoutLocation.OnLoadData();
        this.layoutExamine.OnLoadData();
        this.popupMenu.OnLoadData();
        if (Turn.FocusedCard != null)
        {
            if (!LeanTween.isTweening(Turn.FocusedCard.gameObject))
            {
                Turn.FocusedCard.transform.position = this.layoutLocation.transform.position;
                Turn.FocusedCard.transform.localScale = this.layoutLocation.Scale;
            }
            Turn.FocusedCard.Show(true);
        }
        if (Turn.SummonsMonster == Turn.Card.ID)
        {
            Turn.Card.Decorations.Add("Blueprints/Gui/Vfx_Card_Notice_Summoned", CardSideType.Front, null, 0f);
        }
    }

    private void OnMapToggleButtonPushed()
    {
        if (!base.Paused)
        {
            if (this.mapPanel.Visible && (this.mapPanel.Mode == MapModeType.Look))
            {
                this.mapPanel.Close();
                this.ShowMap(false);
            }
            else
            {
                this.CloseSubWindows();
                this.mapPanel.Mode = MapModeType.Look;
                this.ShowMap(true);
            }
        }
    }

    private void OnMenuButtonPushed()
    {
        if (!base.Paused)
        {
            this.CloseSubWindows();
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnProceedButtonPushed()
    {
        if ((!base.Paused && !UI.Busy) && Turn.IsProceedAllowed())
        {
            if (this.proceedButton.Image == this.proceedButton.GetResourceSprite(0))
            {
                UI.Sound.Play(SoundEffectType.ConfirmProceed);
            }
            if (!this.isMapShowing)
            {
                UI.Busy = true;
                this.CloseSubWindows();
                LeanTween.scale(this.proceedButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedButtonPushed));
            }
            else
            {
                this.mapPanel.EnterSelectedLocation();
                this.ShowMap(false);
                this.Refresh();
            }
        }
    }

    public override void OnSaveData()
    {
        this.layoutDiscard.OnSaveData();
        this.layoutRecharge.OnSaveData();
        this.layoutBury.OnSaveData();
        this.layoutBanish.OnSaveData();
        this.layoutSummoner.OnSaveData();
        this.layoutLocation.OnSaveData();
        this.layoutExamine.OnSaveData();
        this.popupMenu.OnSaveData();
    }

    private void OnShareButtonPressed()
    {
        this.CloseSubWindows();
        Turn.PushReturnState();
        Turn.State = GameStateType.Share;
        this.sharePanel.Show(true);
    }

    private void OnStoreButtonPushed()
    {
        if ((!base.Paused && !Settings.Debug.DemoMode) && !Tutorial.Running)
        {
            this.CloseSubWindows();
            this.Pause(true);
            Game.UI.ShowStoreWindow();
        }
    }

    private void OnSummonButtonPushed()
    {
        if (!base.Paused)
        {
            if (Turn.SummonsType == SummonsType.Target)
            {
                Turn.GotoStateDestination();
            }
            else
            {
                Turn.Proceed();
            }
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.dragRecognizer.enabled = !isPaused;
        this.tapRecognizer.enabled = !isPaused;
        this.diceRecognizer.enabled = !isPaused;
        if (isPaused)
        {
            this.dragRecognizer.Reset();
            this.tapRecognizer.Reset();
            this.diceRecognizer.Reset();
        }
        this.dicePanel.Pause(isPaused);
        this.effectsPanel.Pause(isPaused);
    }

    private void ProceedButtonPushed()
    {
        UI.Busy = false;
        if (Turn.SwitchType == SwitchType.Aid)
        {
            this.ShowProceedButton(false);
            Turn.SwitchType = SwitchType.None;
            Turn.SwitchCharacter(Turn.Current);
        }
        else if (Turn.SwitchType == SwitchType.AidAll)
        {
            this.ShowProceedButton(false);
            if (Turn.Iterators.Current.Next())
            {
                Game.UI.SwitchPanel.Show(true);
            }
            else
            {
                Turn.Iterators.Current.End();
                Game.UI.SwitchPanel.Show(true);
            }
        }
        else
        {
            Tutorial.Notify(TutorialEventType.TurnProceeded);
            Turn.Proceed();
        }
    }

    private Character ProcessCard(Card card, DeckType deckType)
    {
        Character character = Party.Find(card.PlayedOwner);
        if (character == null)
        {
            character = Turn.Character;
        }
        if (card.Played)
        {
            Scenario.Current.OnCardPlayed(card);
            character.OnCardPlayed(card);
            Location.Current.OnCardPlayed(card);
        }
        card.Glow(false);
        card.Clear();
        Deck deck = character.GetDeck(deckType);
        deck.Add(card);
        if (deckType == DeckType.Character)
        {
            if (Turn.RechargePositionType == DeckPositionType.Top)
            {
                deck.Move(deck.Count - 1, 0);
            }
            if (this.layoutRecharge.Deck.Count < 1)
            {
                if (Turn.RechargePositionType == DeckPositionType.Shuffle)
                {
                    deck.Shuffle();
                }
                Turn.RechargePositionType = DeckPositionType.None;
            }
        }
        DeckType type = deckType;
        if (type != DeckType.Bury)
        {
            if (type != DeckType.Character)
            {
                if (type == DeckType.Discard)
                {
                    Scenario.Current.OnCardDiscarded(card);
                    Location.Current.OnCardDiscarded(card);
                    character.OnCardDiscarded(card);
                }
                return character;
            }
            Scenario.Current.OnCardRecharged(card);
            return character;
        }
        card.OnCardBuried(card);
        return character;
    }

    public void ProcessLayoutDecks()
    {
        if (Turn.EmptyLayoutDecks)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                Character character = Party.Characters[i];
                for (int num2 = character.Hand.Count - 1; num2 >= 0; num2--)
                {
                    Card card = character.Hand[num2];
                    if (card.Revealed)
                    {
                        bool played = card.Played;
                        card.Clear();
                        if (played)
                        {
                            Scenario.Current.OnCardPlayed(card);
                            character.OnCardPlayed(card);
                            Location.Current.OnCardPlayed(card);
                        }
                    }
                }
            }
            this.layoutHand.Refresh();
            int count = this.layoutRecharge.Deck.Count;
            for (int j = 0; j < count; j++)
            {
                Card card2 = this.layoutRecharge.Deck[0];
                this.ProcessCard(card2, DeckType.Character);
            }
            this.layoutRecharge.Refresh();
            if (count > 0)
            {
                Turn.RechargeReason = GameReasonType.None;
            }
            for (int k = this.layoutDiscard.Deck.Count - 1; k >= 0; k--)
            {
                Card card3 = this.layoutDiscard.Deck[k];
                this.ProcessCard(card3, DeckType.Discard);
            }
            this.layoutDiscard.Refresh();
            for (int m = this.layoutBury.Deck.Count - 1; m >= 0; m--)
            {
                Card card4 = this.layoutBury.Deck[m];
                this.ProcessCard(card4, DeckType.Bury);
            }
            this.layoutBury.Refresh();
            for (int n = this.layoutBanish.Deck.Count - 1; n >= 0; n--)
            {
                Card card5 = this.layoutBanish.Deck[n];
                bool flag2 = card5.Played;
                Character character2 = Party.Find(card5.PlayedOwner);
                card5.Clear();
                if (flag2)
                {
                    Scenario.Current.OnCardPlayed(card5);
                    character2.OnCardPlayed(card5);
                    Location.Current.OnCardPlayed(card5);
                }
                Campaign.Box.Add(card5, false);
            }
            this.layoutBanish.Refresh();
            this.GlowLayouts(false, null);
        }
    }

    public void ProcessRechargableCards()
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            this.ProcessRechargableCardsInDeck(Party.Characters[i], this.layoutDiscard.Deck);
            this.ProcessRechargableCardsInDeck(Party.Characters[i], this.layoutBury.Deck);
            this.ProcessRechargableCardsInDeck(Party.Characters[i], this.layoutBanish.Deck);
        }
    }

    private void ProcessRechargableCardsInDeck(Character character, Deck deck)
    {
        for (int i = deck.Count - 1; (i >= 0) && (i < deck.Count); i--)
        {
            Card card = deck[i];
            this.ProcessRechargeableCard(character, card);
        }
    }

    public void ProcessRechargeableCard(Character character, Card card)
    {
        if (card.Played && Rules.IsCardRechargable(character, card))
        {
            character.OnCardPlayed(card);
            Location.Current.OnCardPlayed(card);
            Scenario.Current.OnCardPlayed(card);
            character.Recharge.Add(card);
            if (Turn.CheckBoard.Get<bool>("GameStateRecharge_KeepLayout"))
            {
                Turn.CheckBoard.Set<bool>("GameStateRecharge_KeepLayout", false);
            }
        }
    }

    public void Recharge(Card card)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.35f, card, Turn.Character.Deck, this.layoutRecharge, DeckPositionType.Bottom));
    }

    public void Recharge(Card card, DeckPositionType position)
    {
        Game.Instance.StartCoroutine(this.MoveCardCoroutine(0.35f, card, Turn.Character.Deck, this.layoutRecharge, position));
    }

    public override void Refresh()
    {
        if (this.layoutHand.Deck != null)
        {
            for (int j = 0; j < this.layoutHand.Deck.Count; j++)
            {
                this.layoutHand.Deck[j].Show(false);
            }
        }
        for (int i = 0; i < Turn.Character.Deck.Count; i++)
        {
            Turn.Character.Deck[i].transform.position = this.layoutRecharge.transform.position;
            Turn.Character.Deck[i].transform.localScale = this.layoutRecharge.Scale;
        }
        this.layoutHand.Deck = Turn.Character.Hand;
        this.layoutReveal.Deck = Turn.Character.Hand;
        Turn.Character.Deck.Layout = this.layoutRecharge;
        this.layoutRecharge.Counter.Deck = Turn.Character.Deck;
        this.layoutDiscard.Counter.Deck = Turn.Character.Discard;
        this.maxHandSizeLabel.Text = Turn.Character.HandSize.ToString();
        Location.Current.Deck.Layout = this.layoutExplore;
        this.layoutExplore.Deck = Location.Current.Deck;
        this.layoutLocation.Deck = Location.Current.Deck;
        this.layoutExplore.Refresh();
        this.layoutExplore.Counter.Deck = Location.Current.Deck;
        this.locationPanel.Refresh();
        if ((Turn.State == GameStateType.Setup) || (Turn.State == GameStateType.Finish))
        {
            this.ShowExploreButton((Turn.Explore && !Turn.Map) && !this.layoutLocation.Visible);
        }
        bool flag = false;
        if (Location.Current != null)
        {
            for (int k = 0; k < Location.Current.Deck.Count; k++)
            {
                if (Location.Current.Deck[k].Known)
                {
                    flag = true;
                    break;
                }
            }
        }
        this.ShowExamineButton(flag && !Turn.Map);
        this.partyPanel.Refresh();
        this.powersPanel.Refresh();
        this.blessingsPanel.Refresh();
        this.effectsPanel.Refresh();
        this.dicePanel.Refresh();
        this.statusPanel.Refresh();
        this.RefreshCancelButton();
        this.RefreshProceedButton();
        if ((Turn.SwitchType == SwitchType.AidAll) || (Turn.SwitchType == SwitchType.Aid))
        {
            this.ShowProceedAidButton(true);
        }
        this.layoutDiscard.Refresh();
        this.layoutBury.Refresh();
        this.layoutRecharge.Refresh();
        this.layoutBanish.Refresh();
    }

    private void RefreshCancelButton()
    {
        bool isVisible = false;
        if ((((this.layoutDiscard.Deck != null) && (this.layoutDiscard.Deck.Count > 0)) || ((this.layoutRecharge.Deck != null) && (this.layoutRecharge.Deck.Count > 0))) || ((((this.layoutBanish.Deck != null) && (this.layoutBanish.Deck.Count > 0)) || ((this.layoutBury.Deck != null) && (this.layoutBury.Deck.Count > 0))) || (Turn.PeekCancelDestination() != null)))
        {
            isVisible = true;
        }
        for (int i = 0; (i < this.layoutReveal.Deck.Count) && !isVisible; i++)
        {
            if ((this.layoutReveal.Deck[i].Revealed || this.layoutReveal.Deck[i].Displayed) && !this.layoutReveal.Deck[i].Locked)
            {
                isVisible = true;
                break;
            }
        }
        for (int j = 0; (j < Turn.Character.Powers.Count) && !isVisible; j++)
        {
            if ((Turn.IsPowerActive(Turn.Character.Powers[j].ID) && Turn.Character.Powers[j].Cancellable) && !Turn.Character.Powers[j].Automatic)
            {
                isVisible = true;
                break;
            }
        }
        this.cancelButton.Fade(isVisible, 0.1f);
    }

    public void RefreshProceedButton()
    {
        if (this.proceedButton.Visible)
        {
            if (!Rules.IsTurnOwner() && !Rules.IsTurnPassed())
            {
                this.proceedButton.Tint(buttonUnavailableColor);
                VisualEffect.Stop(this.proceedButton.gameObject);
            }
            else
            {
                this.proceedButton.Tint(buttonAvailableColor);
                VisualEffect.Start(this.proceedButton.gameObject);
            }
        }
    }

    public override void Reset()
    {
        this.draggedCard = null;
        this.draggedLayout = null;
        this.prevHoveredLayout = null;
    }

    public void Show(string elementName, bool isVisible)
    {
        Transform transform = base.transform.Find(elementName);
        if (transform != null)
        {
            transform.gameObject.SetActive(isVisible);
        }
    }

    public void ShowBlessingCard(bool isVisible)
    {
        if (isVisible)
        {
            if (Scenario.Current.Discard.Count > 0)
            {
                Card card = Scenario.Current.Discard[0];
                if (this.zoomPanel.Card != card)
                {
                    this.UnZoomCard();
                    card.transform.localScale = Vector3.zero;
                    card.transform.position = this.layoutBlessings.transform.position;
                    card.Show(true);
                    this.ZoomCard(card);
                    UI.Sound.Play(SoundEffectType.OpenBlessingDeck);
                }
            }
        }
        else
        {
            this.UnZoomCard();
        }
    }

    public void ShowCancelButton(bool isVisible)
    {
        this.cancelButton.Fade(isVisible, 0.1f);
    }

    public void ShowCombatCheckAnimation()
    {
        this.combatIntroAnimator.SetTrigger("Show");
        UI.Sound.Play(SoundEffectType.CombatBegin);
    }

    public void ShowConfirmButton(bool isVisible)
    {
        this.layoutExplore.Glow(isVisible);
        this.layoutExplore.Help(isVisible && Rules.IsTurnOwner());
        this.confirmButton.Show(isVisible);
    }

    public void ShowEncounterButton(bool isVisible)
    {
        if (isVisible)
        {
            this.proceedButton.clickSound = this.proceedButton.GetResourceSfx(0);
            if (Turn.Card.IsBane())
            {
                this.proceedButton.Image = this.proceedButton.GetResourceSprite(4);
            }
            else
            {
                this.proceedButton.Image = this.proceedButton.GetResourceSprite(5);
            }
        }
        this.proceedButton.Fade(isVisible, 0.1f);
    }

    public void ShowExamineButton(bool isVisible)
    {
        this.examineButton.Fade(isVisible, 0.15f);
    }

    public void ShowExploreButton(bool isVisible)
    {
        bool flag = (isVisible && (Location.Current.Deck.Count > 0)) && !this.isMapShowing;
        this.layoutExplore.Glow(flag);
        this.layoutExplore.Help(flag && Rules.IsTurnOwner());
        if (isVisible || Turn.Card.IsBlocker())
        {
            this.layoutExplore.Display();
        }
        if (isVisible)
        {
            UI.Sound.Play(SoundEffectType.ExploreAvailable);
        }
    }

    public void ShowMap(bool isVisible)
    {
        if (isVisible)
        {
            this.ShowExploreButton(false);
            this.ShowConfirmButton(false);
            this.ShowExamineButton(false);
        }
        Location.Current.Show(!isVisible);
        if (!isVisible && Location.Current.Closed)
        {
            this.locationClosedOverlay.SetActive(true);
        }
        else
        {
            this.locationClosedOverlay.SetActive(false);
        }
        this.isMapShowing = isVisible;
        this.mapPanel.Show(isVisible);
        if (isVisible)
        {
            this.mapPanel.Recenter();
        }
    }

    public void ShowMapButton(bool isVisible)
    {
        this.mapButton.Fade(isVisible, 0.15f);
    }

    public void ShowProceedAidButton(bool isVisible)
    {
        if (isVisible)
        {
            this.proceedButton.Image = this.proceedButton.GetResourceSprite(1);
            this.proceedButton.clickSound = this.proceedButton.GetResourceSfx(0);
        }
        this.proceedButton.Fade(isVisible, 0.1f);
        this.RefreshProceedButton();
    }

    public void ShowProceedButton(bool isVisible)
    {
        if (isVisible)
        {
            this.proceedButton.Image = this.proceedButton.GetResourceSprite(0);
            this.proceedButton.clickSound = this.proceedButton.GetResourceSfx(0);
        }
        this.proceedButton.Fade(isVisible, 0.1f);
        this.RefreshProceedButton();
    }

    public void ShowProceedDiscardButton(bool isVisible)
    {
        if (isVisible)
        {
            this.proceedButton.Image = this.proceedButton.GetResourceSprite(2);
            this.proceedButton.clickSound = this.proceedButton.GetResourceSfx(1);
        }
        this.proceedButton.Fade(isVisible, 0.1f);
        this.RefreshProceedButton();
    }

    public void ShowProceedEndButton(bool isVisible)
    {
        if (isVisible)
        {
            this.proceedButton.Image = this.proceedButton.GetResourceSprite(3);
            this.proceedButton.clickSound = this.proceedButton.GetResourceSfx(0);
            this.powersPanel.ShowLocationPowerProceedButton(false);
            this.powersPanel.ShowScenarioPowerProceedButton(false);
        }
        this.proceedButton.Fade(isVisible, 0.1f);
        this.RefreshProceedButton();
    }

    public void ShowProceedEndTurnButton(bool isVisible)
    {
        if (Turn.Owner.IsOverHandSize() || Turn.Discard)
        {
            this.ShowProceedDiscardButton(true);
        }
        else
        {
            this.ShowProceedEndButton(true);
        }
    }

    public void ShowShareButton(bool isVisible)
    {
        this.shareButtonAnimator.SetBool("ShowButton", isVisible);
        this.shareButton.Locked = !isVisible;
    }

    public void ShowSummonButton(bool isVisible)
    {
        this.summonButton.Fade(isVisible, 0.15f);
    }

    private void ShowZoomMenu()
    {
        this.zoomPanel.Show(true);
    }

    protected override void Start()
    {
        base.Start();
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy && !this.dicePanel.Rolling)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !this.dicePanel.Rolling)
            {
                this.dragLocation = this.dragRecognizer.touchLocation();
                if (this.draggedCard == null)
                {
                    this.OnGuiDragStart(this.dragLocation);
                }
                else
                {
                    this.OnGuiDrag(this.dragLocation);
                }
            }
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiDragEnd(this.dragLocation);
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.diceRecognizer = new TKDiceRecognizer(base.Camera, Constants.LAYER_MASK_DICE, 2.5f);
        this.diceRecognizer.gestureRecognizedEvent += delegate (TKDiceRecognizer r) {
            if ((!UI.Busy && (this.zoomPanel.Card == null)) && (!this.dicePanel.Rolling && (this.draggedCard == null)))
            {
                this.OnGuiSwipe(this.diceRecognizer.endPoint - this.diceRecognizer.startPoint);
            }
        };
        TouchKit.addGestureRecognizer(this.diceRecognizer);
        this.partyPanel.Initialize();
        this.dicePanel.Initialize();
        this.deathPanel.Initialize();
        this.powersPanel.Initialize();
        this.commandsPanel.Initialize();
        this.messagePanel.Initialize();
        this.locationPanel.Initialize();
        this.skillsPanel.Initialize();
        this.mapPanel.Initialize();
        this.tempClosePanel.Initialize();
        this.characterPanel.Initialize();
        this.layoutExamine.Initialize();
        this.chooseTypePanel.Initialize();
        this.phasesPanel.Initialize();
        this.popupMenu.Initialize();
        this.shadePanel.Initialize();
        this.shufflePanel.Initialize();
        this.targetPanel.Initialize();
        this.closeLocationPanel.Initialize();
        this.blessingsPanel.Initialize();
        this.effectsPanel.Initialize();
        this.villainPanel.Initialize();
        this.statusPanel.Initialize();
        this.helpPanel.Initialize();
        this.zoomPanel.Initialize();
        this.layoutBlessings.Deck = Scenario.Current.Discard;
        this.layoutLocation.Show(false);
        this.layoutLocation.Deck = Location.Current.Deck;
        if (!StartingNewGame)
        {
            this.OnLoadData();
            this.layoutLocation.Refresh();
        }
        StartingNewGame = false;
        for (int i = 0; i < Scenario.Current.LocationCards.Count; i++)
        {
            Scenario.Current.LocationCards[i].Show(false);
        }
        this.shadePanel.Refresh();
        this.layoutExplore.Refresh();
        this.Refresh();
        bool emptyLayoutDecks = Turn.EmptyLayoutDecks;
        Turn.EmptyLayoutDecks = false;
        Turn.State = Turn.State;
        Turn.EmptyLayoutDecks = emptyLayoutDecks;
        this.ShowMap(this.startInMap);
    }

    public float UnZoomCard()
    {
        float num = 0f;
        if (this.zoomPanel.Card != null)
        {
            if (this.zoomPanel.Card.Deck == Scenario.Current.Discard)
            {
                UI.Sound.Play(SoundEffectType.CloseBlessingDeck);
            }
            num = this.zoomPanel.Card.OnGuiZoom(false);
            this.HideZoomMenu();
            this.zoomPanel.Card = null;
            this.draggedCard = null;
            UI.Zoomed = false;
        }
        return num;
    }

    public void Validate()
    {
        if (Rules.IsValidationRequired())
        {
            bool flag = false;
            Turn.Validate = false;
            Turn.Operation = TurnOperationType.Validation;
            UI.Sound.Pause(true);
            flag |= this.layoutBury.Validate(this.layoutBury.Deck);
            flag |= this.layoutDiscard.Validate(this.layoutDiscard.Deck);
            flag |= this.layoutBanish.Validate(this.layoutBanish.Deck);
            flag |= this.layoutRecharge.Validate(this.layoutRecharge.Deck);
            flag |= this.layoutReveal.Validate(null);
            for (int i = 0; i < Turn.Character.Powers.Count; i++)
            {
                if (Turn.IsPowerActive(Turn.Character.Powers[i].ID) && !Turn.Character.Powers[i].IsLegalActivation())
                {
                    Turn.Character.Powers[i].Deactivate();
                }
            }
            Turn.Operation = TurnOperationType.None;
            Turn.Validate = false;
            UI.Sound.Pause(false);
            if (flag)
            {
                this.layoutReveal.Refresh();
                this.layoutHand.Refresh();
            }
        }
    }

    public float ZoomCard(Card card)
    {
        float duration = 0f;
        if (card != null)
        {
            duration = card.OnGuiZoom(true);
            UI.Lock(duration);
            this.zoomedDiceHidden = this.dicePanel.IsDiceVisible();
            if (this.zoomedDiceHidden)
            {
                this.dicePanel.Fade(false, 0.4f);
            }
            LeanTween.delayedCall(duration, new Action(this.ShowZoomMenu));
            this.zoomPanel.Card = card;
            UI.Zoomed = true;
        }
        return duration;
    }

    public GuiPanelMenuPopup Popup =>
        this.popupMenu;

    public static bool StartingNewGame
    {
        [CompilerGenerated]
        get => 
            <StartingNewGame>k__BackingField;
        [CompilerGenerated]
        set
        {
            <StartingNewGame>k__BackingField = value;
        }
    }

    public override WindowType Type =>
        WindowType.Location;

    [CompilerGenerated]
    private sealed class <DrawCardsFromBoxToDeckCoroutine>c__Iterator89 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card[] <$>cards;
        internal Deck <$>deck;
        internal int <$>turn;
        internal GuiWindowLocation <>f__this;
        internal int <i>__0;
        internal int <i>__2;
        internal int <i>__3;
        internal GameObject <vfx>__1;
        internal Card[] cards;
        internal Deck deck;
        internal int turn;

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
                    Turn.Number = this.turn;
                    UI.Window.Refresh();
                    UI.Window.Pause(true);
                    this.<i>__0 = 0;
                    while (this.<i>__0 < this.cards.Length)
                    {
                        if (this.cards[this.<i>__0] != null)
                        {
                            this.cards[this.<i>__0].Show(false);
                        }
                        this.<i>__0++;
                    }
                    this.<vfx>__1 = VisualEffect.ApplyToPlayer(VisualEffectType.CardSummonFromBox, 3f);
                    if (this.<vfx>__1 != null)
                    {
                        this.<vfx>__1.transform.localScale = new Vector3(0.6f + (0.06f * this.cards.Length), 0.6f, 1f);
                        this.<vfx>__1.transform.position = new Vector3(0f, 0f, 0f);
                    }
                    UI.Sound.Play(SoundEffectType.AcquireCardFromBox);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(2f));
                    this.$PC = 1;
                    goto Label_02DE;

                case 1:
                    this.<i>__2 = 0;
                    while (this.<i>__2 < this.cards.Length)
                    {
                        if (this.cards[this.<i>__2] != null)
                        {
                            this.cards[this.<i>__2].transform.localScale = new Vector3(0.6f, 0.6f, 1f);
                            this.cards[this.<i>__2].transform.position = new Vector3(0.3f * this.<i>__2, 0f, 0f);
                            this.cards[this.<i>__2].SortingOrder = this.<i>__2;
                            this.cards[this.<i>__2].Show(CardSideType.Front);
                        }
                        this.<i>__2++;
                    }
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 2;
                    goto Label_02DE;

                case 2:
                    this.<i>__3 = 0;
                    while (this.<i>__3 < this.cards.Length)
                    {
                        if (this.cards[this.<i>__3] != null)
                        {
                            this.deck.Add(this.cards[this.<i>__3]);
                            this.cards[this.<i>__3].Show(CardSideType.Front);
                        }
                        this.<i>__3++;
                    }
                    UI.Window.Pause(false);
                    this.deck.Layout.Refresh();
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_02DE:
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
    private sealed class <DrawFromBoxCoroutine>c__Iterator8A : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal GuiWindowLocation <>f__this;
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
                    this.card.transform.position = this.<>f__this.boxPanel.transform.position;
                    this.card.transform.localScale = new Vector3(this.<>f__this.layoutHand.CardSize, this.<>f__this.layoutHand.CardSize, 1f);
                    this.<>f__this.boxPanel.Show(true);
                    this.<>f__this.boxPanel.PlayAnimation("AddCard", true);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.4f));
                    this.$PC = 1;
                    goto Label_017F;

                case 1:
                    this.<>f__this.Draw(this.card);
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(1f));
                    this.$PC = 2;
                    goto Label_017F;

                case 2:
                    this.<>f__this.boxPanel.Show(false);
                    Turn.Character.Hand.Add(this.card);
                    this.<>f__this.Refresh();
                    this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(0.1f));
                    this.$PC = 3;
                    goto Label_017F;

                case 3:
                    this.$PC = -1;
                    break;
            }
            return false;
        Label_017F:
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
    private sealed class <MoveCardCoroutine>c__Iterator8B : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal Card <$>card;
        internal Deck <$>deck;
        internal float <$>delay;
        internal GuiLayout <$>layout;
        internal DeckPositionType <$>position;
        internal GuiWindowLocation <>f__this;
        internal Vector3[] <curve>__2;
        internal GuiLayout <oldLayout>__0;
        internal float <time>__1;
        internal Card card;
        internal Deck deck;
        internal float delay;
        internal GuiLayout layout;
        internal DeckPositionType position;

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
                    if ((this.card != null) && (this.deck != null))
                    {
                        this.<oldLayout>__0 = this.card.Layout;
                        if (this.card.Deck != null)
                        {
                            this.card.Deck.Remove(this.card);
                        }
                        this.deck.Add(this.card, this.position);
                        this.card.Show(true);
                        if ((this.layout != null) && (this.layout != this.<>f__this.layoutHand))
                        {
                            this.<time>__1 = Geometry.GetTweenTime(this.card.gameObject.transform.position, this.layout.transform.position, this.delay);
                            LeanTween.cancel(this.card.gameObject);
                            this.<curve>__2 = Geometry.GetCurve(this.card.gameObject.transform.position, this.layout.transform.position, 0.5f);
                            this.card.MoveCard(this.<curve>__2, this.<time>__1).setEase(LeanTweenType.easeInOutQuad);
                            LeanTween.scale(this.card.gameObject, this.layout.Scale, this.<time>__1).setEase(LeanTweenType.easeInOutQuad);
                            this.layout.PlayOnCardDroppedSfx();
                            this.$current = Game.Instance.StartCoroutine(this.<>f__this.WaitForTime(this.<time>__1));
                            this.$PC = 1;
                            return true;
                        }
                        break;
                    }
                    goto Label_0249;

                case 1:
                    break;

                default:
                    goto Label_0249;
            }
            this.card.Show(false);
            if (this.<oldLayout>__0 != null)
            {
                this.<oldLayout>__0.Refresh();
            }
            this.layout.Refresh();
            if ((this.layout != null) && (this.layout.CardAction == ActionType.Discard))
            {
                Scenario.Current.OnCardDiscarded(this.card);
                Location.Current.OnCardDiscarded(this.card);
            }
            this.$PC = -1;
        Label_0249:
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

