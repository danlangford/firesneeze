using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiWindowSelectCards : GuiWindow
{
    [Tooltip("reference to the \"all\" button")]
    public GuiButton allCharacterFilterButton;
    [Tooltip("reference to the \"all\" button")]
    public GuiButton allPartyFilterButton;
    public GuiLabel allyCharacterCountLabel;
    [Tooltip("reference to the filter box by allies button")]
    public GuiButton allyCharacterFilterButton;
    public GuiLabel allyPartyCountLabel;
    [Tooltip("reference to the filter box by allies button")]
    public GuiButton allyPartyFilterButton;
    public GuiLabel armorCharacterCountLabel;
    [Tooltip("reference to the filter box by armors button")]
    public GuiButton armorCharacterFilterButton;
    public GuiLabel armorPartyCountLabel;
    [Tooltip("reference to the filter box by armors button")]
    public GuiButton armorPartyFilterButton;
    public GuiLabel blessingCharacterCountLabel;
    [Tooltip("reference to the filter box by blessings button")]
    public GuiButton blessingCharacterFilterButton;
    public GuiLabel blessingPartyCountLabel;
    [Tooltip("reference to the filter box by blessings button")]
    public GuiButton blessingPartyFilterButton;
    [Tooltip("reference to the box deck in our hierarchy")]
    public Deck boxDeck;
    private CardType cardFilter;
    private Card draggedCard;
    private Vector2 dragLocation;
    private TKPanRecognizer dragRecognizer;
    private FingerGesture finger;
    private Card fingerCard;
    private bool fingerGestureStarted;
    private bool[] isHandValid;
    public GuiLabel itemCharacterCountLabel;
    [Tooltip("reference to the filter box by items button")]
    public GuiButton itemCharacterFilterButton;
    public GuiLabel itemPartyCountLabel;
    [Tooltip("reference to the filter box by items button")]
    public GuiButton itemPartyFilterButton;
    [Tooltip("reference to the \"box tray\" in our hierarchy")]
    public GuiLayoutTray layoutBox;
    [Tooltip("reference to the \"hand tray\" in our hierarchy")]
    public GuiLayoutTray layoutHand;
    [Tooltip("reference to the small portrait on the \"all\" button")]
    public GuiImage miniPortrait;
    [Tooltip("reference to the \"character sheet\" panel in scene")]
    public GuiPanel panelCharacter;
    [Tooltip("reference to the \"message panel\" in our hierarchy")]
    public GuiPanelMessage panelMessage;
    [Tooltip("reference to the party panel in this scene")]
    public GuiPanelPartyLine panelParty;
    [Tooltip("reference to the \"real\" proceed button in our hierarchy")]
    public GuiButton proceedButton;
    [Tooltip("reference to the \"disabled\" proceed button in our hierarchy")]
    public GuiButton proceedDisabledButton;
    public GuiLabel spellCharacterCountLabel;
    [Tooltip("reference to the filter box by spells button")]
    public GuiButton spellCharacterFilterButton;
    public GuiLabel spellPartyCountLabel;
    [Tooltip("reference to the filter box by spells button")]
    public GuiButton spellPartyFilterButton;
    private TKTapRecognizer tapRecognizer;
    public GameObject vfxAllyCharacterFilterButtonDown;
    public GameObject vfxAllyCharacterFilterButtonUp;
    public GameObject vfxArmorCharacterFilterButtonDown;
    public GameObject vfxArmorCharacterFilterButtonUp;
    public GameObject vfxBlessingCharacterFilterButtonDown;
    public GameObject vfxBlessingCharacterFilterButtonUp;
    public GameObject vfxItemCharacterFilterButtonDown;
    public GameObject vfxItemCharacterFilterButtonUp;
    public GameObject vfxSpellCharacterFilterButtonDown;
    public GameObject vfxSpellCharacterFilterButtonUp;
    public GameObject vfxWeaponCharacterFilterButtonDown;
    public GameObject vfxWeaponCharacterFilterButtonUp;
    public GuiLabel weaponCharacterCountLabel;
    [Tooltip("reference to the filter box by weapons button")]
    public GuiButton weaponCharacterFilterButton;
    public GuiLabel weaponPartyCountLabel;
    [Tooltip("reference to the filter box by weapons button")]
    public GuiButton weaponPartyFilterButton;
    private Card zoomedCard;

    public override void Close()
    {
        base.Close();
        for (int i = 0; i < Campaign.Distributions.Count; i++)
        {
            CardIdentity identity = new CardIdentity(Campaign.Distributions[i], null);
            Campaign.Box.Push(identity, true);
        }
        Campaign.Distributions.Clear();
        for (int j = 0; j < Party.Characters.Count; j++)
        {
            for (int m = 0; m < Party.Characters[j].Deck.Count; m++)
            {
                VisualEffect.Remove(Party.Characters[j].Deck[m], VisualEffectType.GlowSpecial);
            }
        }
        this.LockAllCards(false);
        this.layoutHand.Show(false);
        this.layoutBox.Show(false);
        for (int k = 0; k < Party.Characters.Count; k++)
        {
            if (Vault.Contains(Party.Characters[k].NickName))
            {
                Vault.Add(Party.Characters[k].NickName, Party.Characters[k]);
            }
        }
    }

    [DebuggerHidden]
    private IEnumerator CreateCards(CardType type) => 
        new <CreateCards>c__Iterator91 { 
            type = type,
            <$>type = type,
            <>f__this = this
        };

    private bool DropCardOnGive(Card card, GuiLayoutPartyLine layout)
    {
        bool flag = false;
        if (card.Deck != Turn.Character.Deck)
        {
            flag = true;
        }
        bool flag2 = layout.OnGuiDrop(card);
        if (flag2)
        {
            this.panelParty.ShowPoofEffect(layout.ID);
            this.VerifyCardCounts(Party.Find(layout.ID));
            if (flag)
            {
                this.OnCardDrawn(card);
            }
            this.layoutHand.Refresh();
            this.layoutBox.Refresh();
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
            this.UpdateProceedButton();
            this.UpdateMessageLine();
        }
        return flag2;
    }

    private bool DropCardOnLayout(Card card, GuiLayout layout, Vector2 touchPos)
    {
        if ((card.Deck == this.boxDeck) && (layout == this.layoutBox))
        {
            return false;
        }
        if ((layout == this.layoutHand) && (card.Deck == Turn.Character.Deck))
        {
            layout.InsertAtDropPosition(this.draggedCard);
        }
        if ((layout == this.layoutHand) && (card.Deck != Turn.Character.Deck))
        {
            this.OnCardDrawn(this.draggedCard);
            layout.Deck.Add(this.draggedCard);
            VisualEffect.Remove(card, VisualEffectType.GlowSpecial);
        }
        if (layout == this.layoutBox)
        {
            this.OnCardDiscarded(this.draggedCard);
            Card card2 = CardTable.Create(this.draggedCard.ID);
            card2.SortingOrder = this.draggedCard.SortingOrder;
            card2.transform.position = this.draggedCard.transform.position;
            card2.transform.localScale = layout.Scale;
            layout.Deck.Add(card2);
            layout.Deck.Sort();
            if (!card2.HasTrait(TraitType.Basic))
            {
                VisualEffect.Apply(card, VisualEffectType.GlowSpecial);
            }
        }
        this.layoutHand.Refresh();
        this.layoutBox.Refresh();
        this.UpdatePartyCharacters();
        this.UpdateCardCounters();
        this.UpdateProceedButton();
        this.UpdateMessageLine();
        return true;
    }

    private string[] GetBoxCardList(CardType type)
    {
        if (this.Mode == ModeType.Build)
        {
            return this.GetBuildingCards(type);
        }
        if (this.Mode == ModeType.Distribute)
        {
            return this.GetDistributionCards(type);
        }
        if (this.Mode == ModeType.Repair)
        {
            return this.GetRepairCards(Turn.Character, type);
        }
        return null;
    }

    private string[] GetBuildingCards(CardType type)
    {
        List<string> list = new List<string>(Campaign.Box.GetCardList(type, CardRankType.Basic));
        list.AddRange(this.GetDistributionCards(type));
        List<string> cards = Collection.GetCards("B");
        for (int i = 0; i < cards.Count; i++)
        {
            if (CardTable.LookupCardType(cards[i]) == type)
            {
                TraitType[] cardTraits = CardTable.LookupCardTraits(cards[i]);
                if (cardTraits != null)
                {
                    for (int j = 0; j < cardTraits.Length; j++)
                    {
                        if (cardTraits[j] == TraitType.Basic)
                        {
                            list.Add(cards[i]);
                            break;
                        }
                    }
                }
            }
        }
        return list.ToArray();
    }

    private string GetCharacterCardCount(CardType type)
    {
        int num = Turn.Character.Deck.Filter(type);
        int cardRank = Turn.Character.GetCardRank(type);
        if (num != cardRank)
        {
            this.isHandValid[Turn.Number] = false;
            this.panelParty.SetCharacterMarker(Turn.Character.ID, true);
            object[] objArray1 = new object[] { "<color=red>", num, "</color> / ", cardRank };
            return string.Concat(objArray1);
        }
        object[] objArray2 = new object[] { "<color=white>", num, "</color> / ", cardRank };
        return string.Concat(objArray2);
    }

    private string[] GetDistributionCards(CardType cardType)
    {
        List<string> list = new List<string>(Campaign.Distributions.Count);
        for (int i = 0; i < Campaign.Distributions.Count; i++)
        {
            string item = Campaign.Distributions[i];
            if (cardType == CardType.None)
            {
                list.Add(item);
            }
            else
            {
                CardType type = CardTable.LookupCardType(item);
                if (type == cardType)
                {
                    list.Add(item);
                }
                else if ((type == CardType.Loot) && (CardTable.LookupCardSubType(item) == cardType))
                {
                    list.Add(item);
                }
            }
        }
        return list.ToArray();
    }

    private string GetPartyCardCount(CardType type)
    {
        int num = 0;
        int num2 = 0;
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            num += Party.Characters[i].Deck.Filter(type);
            num2 += Party.Characters[i].GetCardRank(type);
        }
        if (num != num2)
        {
            object[] objArray1 = new object[] { "<color=red>", num, "</color> / ", num2 };
            return string.Concat(objArray1);
        }
        object[] objArray2 = new object[] { "<color=white>", num, "</color> / ", num2 };
        return string.Concat(objArray2);
    }

    private string[] GetRepairCards(Character character, CardType type)
    {
        int num = character.Deck.Filter(type);
        int cardRank = character.GetCardRank(type);
        if (num >= cardRank)
        {
            return null;
        }
        if (Game.GameMode == GameModeType.Story)
        {
            if (Campaign.IsAdventureComplete("AD16_SpiresOfXinShalast"))
            {
                return Campaign.Box.GetCardList(type, 4);
            }
            if (Campaign.IsAdventureComplete("AD15_SinsOfTheSaviors"))
            {
                return Campaign.Box.GetCardList(type, 3);
            }
            if (Campaign.IsAdventureComplete("AD14_FortressOfTheStoneGiants"))
            {
                return Campaign.Box.GetCardList(type, 2);
            }
            if (Campaign.IsAdventureComplete("AD13_TheHookMountainMassacre"))
            {
                return Campaign.Box.GetCardList(type, 1);
            }
        }
        return Campaign.Box.GetCardList(type, CardRankType.Basic);
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

    private void GlowButtons(CardType type)
    {
        this.allCharacterFilterButton.Glow(false);
        this.allPartyFilterButton.Glow(false);
        this.weaponCharacterFilterButton.Glow(type == CardType.Weapon);
        this.weaponPartyFilterButton.Glow(type == CardType.Weapon);
        this.spellCharacterFilterButton.Glow(type == CardType.Spell);
        this.spellPartyFilterButton.Glow(type == CardType.Spell);
        this.itemCharacterFilterButton.Glow(type == CardType.Item);
        this.itemPartyFilterButton.Glow(type == CardType.Item);
        this.armorCharacterFilterButton.Glow(type == CardType.Armor);
        this.armorPartyFilterButton.Glow(type == CardType.Armor);
        this.allyCharacterFilterButton.Glow(type == CardType.Ally);
        this.allyPartyFilterButton.Glow(type == CardType.Ally);
        this.blessingCharacterFilterButton.Glow(type == CardType.Blessing);
        this.blessingPartyFilterButton.Glow(type == CardType.Blessing);
    }

    private bool IsDragAllowed(Card card) => 
        ((card != null) && !card.Locked);

    private bool IsDropAllowed(Card card, GuiLayout layout) => 
        !card.Locked;

    private bool IsGiveAllowed(Card card, GuiLayoutPartyLine layout)
    {
        if ((this.Mode != ModeType.Build) && (this.Mode != ModeType.Distribute))
        {
            return false;
        }
        return true;
    }

    private bool IsHandBelowCapacity(int index, CardType type)
    {
        int num = Party.Characters[index].Deck.Filter(type);
        int cardRank = Party.Characters[index].GetCardRank(type);
        return (num <= cardRank);
    }

    private bool IsHandValid(int index)
    {
        if (!this.IsHandValid(index, CardType.Weapon))
        {
            return false;
        }
        if (!this.IsHandValid(index, CardType.Spell))
        {
            return false;
        }
        if (!this.IsHandValid(index, CardType.Item))
        {
            return false;
        }
        if (!this.IsHandValid(index, CardType.Ally))
        {
            return false;
        }
        if (!this.IsHandValid(index, CardType.Blessing))
        {
            return false;
        }
        if (!this.IsHandValid(index, CardType.Armor))
        {
            return false;
        }
        return true;
    }

    private bool IsHandValid(int index, CardType type)
    {
        if ((((type != CardType.Weapon) && (type != CardType.Spell)) && ((type != CardType.Item) && (type != CardType.Ally))) && ((type != CardType.Blessing) && (type != CardType.Armor)))
        {
            return true;
        }
        int num = Party.Characters[index].Deck.Filter(type);
        int cardRank = Party.Characters[index].GetCardRank(type);
        return (num == cardRank);
    }

    private bool IsPartyValid(CardType type)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            if (!this.IsHandValid(i, type))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsProceedAllowed()
    {
        if (this.Mode == ModeType.Distribute)
        {
            for (int j = 0; j < Party.Characters.Count; j++)
            {
                if (!this.IsHandBelowCapacity(j, CardType.Weapon))
                {
                    return false;
                }
                if (!this.IsHandBelowCapacity(j, CardType.Spell))
                {
                    return false;
                }
                if (!this.IsHandBelowCapacity(j, CardType.Item))
                {
                    return false;
                }
                if (!this.IsHandBelowCapacity(j, CardType.Ally))
                {
                    return false;
                }
                if (!this.IsHandBelowCapacity(j, CardType.Blessing))
                {
                    return false;
                }
                if (!this.IsHandBelowCapacity(j, CardType.Armor))
                {
                    return false;
                }
            }
            for (int k = 0; k < Party.Characters.Count; k++)
            {
                for (int m = 0; m < Campaign.Distributions.Count; m++)
                {
                    CardType cardType = CardTable.LookupCardType(Campaign.Distributions[m]);
                    if (!this.IsHandValid(k, cardType))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        for (int i = 0; i < this.isHandValid.Length; i++)
        {
            if (!this.isHandValid[i])
            {
                return false;
            }
        }
        return true;
    }

    private void LockAllCards(bool isLocked)
    {
        for (int i = 0; i < Party.Characters.Count; i++)
        {
            for (int j = 0; j < Party.Characters[i].Deck.Count; j++)
            {
                if (isLocked)
                {
                    Party.Characters[i].Deck[j].Locked = true;
                    Party.Characters[i].Deck[j].Tint(Color.gray);
                }
                else
                {
                    Party.Characters[i].Deck[j].Locked = false;
                    Party.Characters[i].Deck[j].Tint(Color.white);
                }
            }
        }
    }

    private void OnCardDiscarded(Card card)
    {
        if (this.Mode == ModeType.Build)
        {
            if (!card.HasTrait(TraitType.Basic))
            {
                Campaign.Distributions.Add(card.ID);
                this.layoutHand.Deck.Remove(card);
            }
            else
            {
                Campaign.Box.Add(card, false);
            }
        }
        if (this.Mode == ModeType.Distribute)
        {
            Campaign.Distributions.Add(card.ID);
            this.layoutHand.Deck.Remove(card);
        }
        if (this.Mode == ModeType.Repair)
        {
            Campaign.Box.Add(card, false);
        }
    }

    private void OnCardDrawn(Card card)
    {
        if (this.Mode == ModeType.Build)
        {
            if (Campaign.Distributions.Contains(card.ID))
            {
                Campaign.Distributions.Remove(card.ID);
            }
            else
            {
                Campaign.Box.Remove(card);
            }
        }
        if (this.Mode == ModeType.Distribute)
        {
            Campaign.Distributions.Remove(card.ID);
        }
        if (this.Mode == ModeType.Repair)
        {
            Campaign.Box.Remove(card);
        }
    }

    private void OnCardFilterAllButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.GlowButtons(CardType.None);
            this.allCharacterFilterButton.Glow(true);
            this.allPartyFilterButton.Glow(true);
            this.SetupBoxDeck(CardType.None);
            this.SetupHandDeck(CardType.None);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterAlliesButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Ally);
            this.SetupHandDeck(CardType.Ally);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterArmorsButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Armor);
            this.SetupHandDeck(CardType.Armor);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterBlessingsButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Blessing);
            this.SetupHandDeck(CardType.Blessing);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterItemsButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Item);
            this.SetupHandDeck(CardType.Item);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterSpellsButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Spell);
            this.SetupHandDeck(CardType.Spell);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCardFilterWeaponsButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            this.SetupBoxDeck(CardType.Weapon);
            this.SetupHandDeck(CardType.Weapon);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
    }

    private void OnCharacterSheetButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            UI.Window.Pause(true);
            UI.Window.Show(false);
            this.panelCharacter.Show(true);
        }
    }

    private void OnCharacterSwitch()
    {
        this.Refresh();
        if (this.Mode == ModeType.Repair)
        {
            this.SetupBoxDeck(this.cardFilter);
            this.UpdatePartyCharacters();
            this.UpdateCardCounters();
        }
        if (Game.GameType == GameType.LocalMultiPlayer)
        {
            Game.UI.SwitchPanel.Show(true);
        }
    }

    private void OnGuiDrag(Vector2 deltaTranslation)
    {
        if ((this.finger.State == FingerGesture.FingerState.Drag) && (this.draggedCard != null))
        {
            Vector3 screenPoint = (Vector3) (base.WorldToScreenPoint(this.draggedCard.transform.position) + deltaTranslation);
            this.draggedCard.transform.position = (Vector3) base.ScreenToWorldPoint(screenPoint);
        }
    }

    private void OnGuiDragEnd(Vector2 touchPos)
    {
        if (this.finger.State == FingerGesture.FingerState.Drag)
        {
            bool isValid = false;
            if (this.draggedCard != null)
            {
                RaycastHit2D hitd = Physics2D.Raycast(base.ScreenToWorldPoint(touchPos), Vector2.zero, float.PositiveInfinity, Constants.LAYER_MASK_DEFAULT);
                if (hitd != 0)
                {
                    GuiLayoutTray component = hitd.collider.transform.GetComponent<GuiLayoutTray>();
                    if (component != null)
                    {
                        isValid = this.IsDropAllowed(this.draggedCard, component) && this.DropCardOnLayout(this.draggedCard, component, touchPos);
                    }
                    else
                    {
                        GuiLayoutPartyLine layout = hitd.collider.transform.GetComponent<GuiLayoutPartyLine>();
                        if (layout != null)
                        {
                            isValid = this.IsGiveAllowed(this.draggedCard, layout) && this.DropCardOnGive(this.draggedCard, layout);
                        }
                    }
                }
                this.draggedCard.OnGuiDrop(isValid);
                UI.Lock(0.2f);
            }
        }
        this.fingerGestureStarted = false;
        this.fingerCard = null;
        this.draggedCard = null;
        this.ResetFingerState();
    }

    private void OnGuiDragStart(Vector2 touchPos)
    {
        if (!this.fingerGestureStarted)
        {
            this.fingerGestureStarted = true;
            this.fingerCard = this.GetTopCard(touchPos);
        }
        this.finger.Calculate(touchPos);
        if ((this.finger.State == FingerGesture.FingerState.Drag) && this.IsDragAllowed(this.fingerCard))
        {
            this.draggedCard = this.fingerCard;
            if (this.draggedCard != null)
            {
                this.draggedCard.OnGuiDrag();
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

    private void OnMenuButtonPushed()
    {
        if ((!UI.Busy && !UI.Window.Paused) && !UI.Zoomed)
        {
            Game.UI.OptionsPanel.Show(true);
        }
    }

    private void OnProceedButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.proceedButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedButtonPushed));
        }
    }

    private void OnProceedDisabledButtonPushed()
    {
        if (!UI.Busy && !UI.Window.Paused)
        {
            UI.Busy = true;
            LeanTween.scale(this.proceedDisabledButton.gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setLoopPingPong().setLoopCount(2).setOnComplete(new Action(this.ProceedDisabledButtonPushed));
        }
    }

    public override void Pause(bool isPaused)
    {
        base.Pause(isPaused);
        this.tapRecognizer.enabled = !isPaused;
        this.dragRecognizer.enabled = !isPaused;
        this.panelParty.Pause(isPaused);
        this.layoutHand.Pause(isPaused);
        this.layoutBox.Pause(isPaused);
    }

    private void ProceedButtonPushed()
    {
        if (this.Mode == ModeType.Distribute)
        {
            Game.Network.OnSalvageCards(this.boxDeck);
            this.Mode = ModeType.Repair;
            if (!this.IsProceedAllowed())
            {
                CardType type = this.SelectRepairType(this.cardFilter);
                this.cardFilter = CardType.None;
                this.SetupBoxDeck(type);
                this.SetupHandDeck(type);
                this.UpdateProceedButton();
                this.UpdatePartyCharacters();
                this.UpdateCardCounters();
                this.LockAllCards(true);
                UI.Busy = false;
                return;
            }
        }
        WindowType exitScene = ExitScene;
        if (exitScene == WindowType.Scenario)
        {
            Game.UI.ShowSetupScene();
        }
        else if (exitScene == WindowType.Quest)
        {
            Game.UI.ShowQuestScene();
        }
        else if (Game.GameMode == GameModeType.Story)
        {
            Game.UI.ShowAdventureScene();
        }
        else
        {
            Game.UI.ShowQuestScene();
        }
        ExitScene = WindowType.None;
    }

    private void ProceedDisabledButtonPushed()
    {
        UI.Busy = false;
        this.panelMessage.Show(StringTableManager.GetHelperText(0x36));
    }

    public override void Refresh()
    {
        this.miniPortrait.Image = Turn.Character.PortraitSmall;
        for (int i = 0; i < this.layoutHand.Deck.Count; i++)
        {
            this.layoutHand.Deck[i].Show(false);
        }
        this.layoutHand.Animations = false;
        this.layoutHand.Deck = Turn.Character.Deck;
        this.layoutHand.Refresh();
        this.layoutHand.Animations = true;
        for (int j = 0; j < this.layoutHand.Deck.Count; j++)
        {
            Card card = this.layoutHand.Deck[j];
            if (!Turn.Character.HasStarterCard(card.ID))
            {
                VisualEffect.Apply(card, VisualEffectType.GlowSpecial);
            }
        }
        this.UpdateCardCounters();
        this.UpdateProceedButton();
        this.UpdateMessageLine();
    }

    private void ResetFingerState()
    {
        this.finger.Reset();
        this.layoutBox.Pause(true);
        this.layoutHand.Pause(true);
    }

    private CardType SelectRepairType(CardType type)
    {
        if (!this.IsHandValid(Turn.Number, type))
        {
            return type;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Weapon))
        {
            return CardType.Weapon;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Spell))
        {
            return CardType.Spell;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Armor))
        {
            return CardType.Armor;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Ally))
        {
            return CardType.Ally;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Item))
        {
            return CardType.Item;
        }
        if (!this.IsHandValid(Turn.Number, CardType.Blessing))
        {
            return CardType.Blessing;
        }
        return CardType.None;
    }

    private ModeType SelectStartMode()
    {
        if (Scenario.Current == null)
        {
            return ModeType.Build;
        }
        if (!Campaign.Started)
        {
            return ModeType.Build;
        }
        return ModeType.Distribute;
    }

    private void SetupBoxDeck(CardType type)
    {
        if (this.cardFilter != type)
        {
            this.cardFilter = type;
            this.GlowButtons(type);
            for (int i = this.boxDeck.Count - 1; i >= 0; i--)
            {
                this.boxDeck.Destroy(this.boxDeck[i].ID);
            }
            this.layoutBox.Reset();
            this.layoutBox.Refresh();
            if ((type != CardType.None) || (this.Mode == ModeType.Distribute))
            {
                base.StartCoroutine(this.CreateCards(type));
            }
        }
    }

    private void SetupHandDeck(CardType type)
    {
        for (int i = 0; i < this.layoutHand.Deck.Count; i++)
        {
            this.layoutHand.Deck[i].Show(false);
        }
        this.layoutHand.Filter.CardTypes[0] = type;
        this.layoutHand.Animations = false;
        this.layoutHand.Reset();
        this.layoutHand.Refresh();
        this.layoutHand.Animations = true;
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.panelParty.Show(isVisible);
    }

    protected override void Start()
    {
        base.Start();
        if (ExitScene == WindowType.Reward)
        {
            Game.Save();
            Game.Synchronize();
        }
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.gestureRecognizedEvent += delegate (TKTapRecognizer r) {
            if (!UI.Busy)
            {
                this.OnGuiTap(this.tapRecognizer.touchLocation());
            }
        };
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.dragRecognizer = new TKPanRecognizer(Device.GetMinimumDragDistance());
        this.dragRecognizer.zIndex = 1;
        this.dragRecognizer.gestureRecognizedEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Zoomed)
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
        };
        this.dragRecognizer.gestureCompleteEvent += delegate (TKPanRecognizer r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                this.OnGuiDragEnd(this.dragLocation);
            }
        };
        TouchKit.addGestureRecognizer(this.dragRecognizer);
        this.dragRecognizer.enabled = true;
        this.finger = new FingerGesture();
        this.finger.gestureRecognizedEvent += delegate (FingerGesture r) {
            if (!UI.Busy && !UI.Zoomed)
            {
                if (this.finger.State == FingerGesture.FingerState.Slide)
                {
                    this.layoutBox.Pause(false);
                    this.layoutHand.Pause(false);
                }
                if (this.finger.State == FingerGesture.FingerState.Drag)
                {
                    this.layoutBox.Pause(true);
                    this.layoutHand.Pause(true);
                }
            }
        };
        this.Mode = this.SelectStartMode();
        this.layoutHand.Initialize();
        this.layoutBox.Initialize();
        this.panelParty.Initialize();
        this.panelMessage.Initialize();
        this.panelCharacter.Initialize();
        Party.Audit();
        this.isHandValid = new bool[Party.Characters.Count];
        for (int i = 0; i < this.isHandValid.Length; i++)
        {
            this.isHandValid[i] = this.IsHandValid(i);
            this.panelParty.SetCharacterMarker(Party.Characters[i].ID, !this.isHandValid[i]);
        }
        this.layoutHand.Deck = Turn.Character.Deck;
        this.layoutHand.Filter = new CardFilter();
        this.layoutHand.Filter.CardTypes = new CardType[] { CardType.Weapon };
        this.layoutHand.Show(true);
        this.layoutHand.Refresh();
        this.layoutBox.Deck = this.boxDeck;
        this.cardFilter = CardType.None;
        this.SetupBoxDeck(CardType.Weapon);
        this.layoutBox.Show(true);
        this.layoutBox.Refresh();
        this.Refresh();
        this.layoutHand.Animations = true;
        this.layoutBox.Animations = true;
        this.UpdateCardCounters();
        this.UpdateProceedButton();
        this.UpdateMessageLine();
        this.ResetFingerState();
        Tutorial.Notify(TutorialEventType.ScreenCardSelectionShown);
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
        UI.Busy = false;
        UI.Zoomed = false;
        this.zoomedCard = null;
        this.draggedCard = null;
    }

    private void UpdateCardCounterIcon(CardType type, GuiButton button, GameObject vfxUp, GameObject vfxDown)
    {
        int num = Turn.Character.Deck.Filter(type);
        int cardRank = Turn.Character.GetCardRank(type);
        vfxUp.SetActive(num < cardRank);
        vfxDown.SetActive(num > cardRank);
    }

    private void UpdateCardCounterIcons()
    {
        this.UpdateCardCounterIcon(CardType.Weapon, this.weaponCharacterFilterButton, this.vfxWeaponCharacterFilterButtonUp, this.vfxWeaponCharacterFilterButtonDown);
        this.UpdateCardCounterIcon(CardType.Spell, this.weaponCharacterFilterButton, this.vfxSpellCharacterFilterButtonUp, this.vfxSpellCharacterFilterButtonDown);
        this.UpdateCardCounterIcon(CardType.Item, this.weaponCharacterFilterButton, this.vfxItemCharacterFilterButtonUp, this.vfxItemCharacterFilterButtonDown);
        this.UpdateCardCounterIcon(CardType.Ally, this.weaponCharacterFilterButton, this.vfxAllyCharacterFilterButtonUp, this.vfxAllyCharacterFilterButtonDown);
        this.UpdateCardCounterIcon(CardType.Blessing, this.weaponCharacterFilterButton, this.vfxBlessingCharacterFilterButtonUp, this.vfxBlessingCharacterFilterButtonDown);
        this.UpdateCardCounterIcon(CardType.Armor, this.weaponCharacterFilterButton, this.vfxArmorCharacterFilterButtonUp, this.vfxArmorCharacterFilterButtonDown);
    }

    private void UpdateCardCounters()
    {
        this.isHandValid[Turn.Number] = true;
        this.panelParty.SetCharacterMarker(Turn.Character.ID, false);
        this.weaponCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Weapon);
        this.weaponPartyCountLabel.Text = this.GetPartyCardCount(CardType.Weapon);
        this.spellCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Spell);
        this.spellPartyCountLabel.Text = this.GetPartyCardCount(CardType.Spell);
        this.itemCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Item);
        this.itemPartyCountLabel.Text = this.GetPartyCardCount(CardType.Item);
        this.allyCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Ally);
        this.allyPartyCountLabel.Text = this.GetPartyCardCount(CardType.Ally);
        this.blessingCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Blessing);
        this.blessingPartyCountLabel.Text = this.GetPartyCardCount(CardType.Blessing);
        this.armorCharacterCountLabel.Text = this.GetCharacterCardCount(CardType.Armor);
        this.armorPartyCountLabel.Text = this.GetPartyCardCount(CardType.Armor);
        this.UpdateCardCounterIcons();
    }

    private void UpdateMessageLine()
    {
        if (this.isHandValid[Turn.Number])
        {
            this.panelMessage.Show(StringTableManager.GetHelperText(0x34));
        }
        else
        {
            this.panelMessage.Show(StringTableManager.GetHelperText(0x35));
        }
    }

    private void UpdatePartyCharacters()
    {
        if (this.Mode == ModeType.Repair)
        {
            for (int i = 0; i < Party.Characters.Count; i++)
            {
                if (this.IsHandValid(i))
                {
                    this.panelParty.SetCharacterTint(Party.Characters[i].ID, Color.gray);
                }
            }
            this.weaponCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Weapon));
            this.weaponPartyFilterButton.Disable(this.IsPartyValid(CardType.Weapon));
            this.spellCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Spell));
            this.spellPartyFilterButton.Disable(this.IsPartyValid(CardType.Spell));
            this.itemCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Item));
            this.itemPartyFilterButton.Disable(this.IsPartyValid(CardType.Item));
            this.allyCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Ally));
            this.allyPartyFilterButton.Disable(this.IsPartyValid(CardType.Ally));
            this.blessingCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Blessing));
            this.blessingPartyFilterButton.Disable(this.IsPartyValid(CardType.Blessing));
            this.armorCharacterFilterButton.Disable(this.IsHandValid(Turn.Number, CardType.Armor));
            this.armorPartyFilterButton.Disable(this.IsPartyValid(CardType.Armor));
        }
    }

    private void UpdateProceedButton()
    {
        bool isVisible = this.IsProceedAllowed();
        this.proceedButton.Show(isVisible);
        this.proceedDisabledButton.Show(!isVisible);
    }

    private void VerifyCardCount(Character character, CardType type)
    {
        int num = character.Deck.Filter(type);
        int cardRank = character.GetCardRank(type);
        if (num != cardRank)
        {
            int index = Party.IndexOf(character.ID);
            this.isHandValid[index] = false;
            this.panelParty.SetCharacterMarker(character.ID, true);
        }
    }

    private void VerifyCardCounts(Character character)
    {
        this.VerifyCardCount(character, CardType.Weapon);
        this.VerifyCardCount(character, CardType.Spell);
        this.VerifyCardCount(character, CardType.Item);
        this.VerifyCardCount(character, CardType.Ally);
        this.VerifyCardCount(character, CardType.Blessing);
        this.VerifyCardCount(character, CardType.Armor);
    }

    private void ZoomCard(Card card)
    {
        if (card != null)
        {
            UI.Busy = true;
            UI.Zoomed = true;
            float duration = card.OnGuiZoom(true);
            this.zoomedCard = card;
            UI.Lock(duration);
        }
    }

    public static WindowType ExitScene
    {
        [CompilerGenerated]
        get => 
            <ExitScene>k__BackingField;
        [CompilerGenerated]
        set
        {
            <ExitScene>k__BackingField = value;
        }
    }

    private ModeType Mode { get; set; }

    public override WindowType Type =>
        WindowType.SelectCards;

    [CompilerGenerated]
    private sealed class <CreateCards>c__Iterator91 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal CardType <$>type;
        internal GuiWindowSelectCards <>f__this;
        internal Card <card>__2;
        internal string[] <cards>__0;
        internal int <i>__1;
        internal CardType type;

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
                    this.<>f__this.Pause(true);
                    UI.Busy = true;
                    Game.UI.BusyBox.Center(-3f);
                    Game.UI.BusyBox.Show(true);
                    Resources.UnloadUnusedAssets();
                    this.<cards>__0 = this.<>f__this.GetBoxCardList(this.type);
                    if (this.<cards>__0 == null)
                    {
                        goto Label_0100;
                    }
                    this.<i>__1 = 0;
                    break;

                case 1:
                    this.<i>__1++;
                    break;

                default:
                    goto Label_0180;
            }
            if (this.<i>__1 < this.<cards>__0.Length)
            {
                this.<card>__2 = CardTable.Create(this.<cards>__0[this.<i>__1]);
                this.<>f__this.boxDeck.Add(this.<card>__2);
                Game.UI.BusyBox.Tick();
                this.$current = new WaitForEndOfFrame();
                this.$PC = 1;
                return true;
            }
        Label_0100:
            this.<>f__this.layoutBox.Animations = false;
            this.<>f__this.layoutBox.Deck.Sort();
            this.<>f__this.layoutBox.Reset();
            this.<>f__this.layoutBox.Refresh();
            this.<>f__this.layoutBox.Animations = true;
            this.<>f__this.Pause(false);
            UI.Busy = false;
            Game.UI.BusyBox.Show(false);
            this.$PC = -1;
        Label_0180:
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

    private enum ModeType
    {
        Build,
        Distribute,
        Repair
    }
}

