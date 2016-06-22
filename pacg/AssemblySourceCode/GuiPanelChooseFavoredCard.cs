using System;
using UnityEngine;

public class GuiPanelChooseFavoredCard : GuiPanel
{
    [Tooltip("reference to the ally button in our hierarchy")]
    public GuiButton AllyButton;
    [Tooltip("reference to the ally button's disabled overlay in our hierarchy")]
    public GameObject AllyButtonDisabledIcon;
    [Tooltip("reference to the armor button in our hierarchy")]
    public GuiButton ArmorButton;
    [Tooltip("reference to the armor button's disabled overlay in our hierarchy")]
    public GameObject ArmorButtonDisabledIcon;
    [Tooltip("reference to the blessing button in our hierarchy")]
    public GuiButton BlessingButton;
    [Tooltip("reference to the blessing button's disabled overlay in our hierarchy")]
    public GameObject BlessingButtonDisabledIcon;
    private bool isBusy;
    [Tooltip("reference to the item button in our hierarchy")]
    public GuiButton ItemButton;
    [Tooltip("reference to the item button's disabled overlay in our hierarchy")]
    public GameObject ItemButtonDisabledIcon;
    [Tooltip("reference to the loot button in our hierarchy")]
    public GuiButton LootButton;
    [Tooltip("reference to the loot button's disabled overlay in our hierarchy")]
    public GameObject LootButtonDisabledIcon;
    private Character myCharacter;
    [Tooltip("reference to the Animator component in our hierarchy")]
    public Animator PanelAnimator;
    [Tooltip("reference to the portrait sprite in our hierarchy")]
    public GuiImage Portrait;
    [Tooltip("reference to the spell button in our hierarchy")]
    public GuiButton SpellButton;
    [Tooltip("reference to the spell button's disabled overlay in our hierarchy")]
    public GameObject SpellButtonDisabledIcon;
    [Tooltip("reference to the weapon button in our hierarchy")]
    public GuiButton WeaponButton;
    [Tooltip("reference to the weapon button's disabled overlay in our hierarchy")]
    public GameObject WeaponButtonDisabledIcon;

    private void ConfigureButtonStates(Character character)
    {
        int num = character.Deck.Filter(CardType.Weapon);
        this.WeaponButton.Disable(num <= 0);
        this.WeaponButtonDisabledIcon.SetActive(num <= 0);
        num = character.Deck.Filter(CardType.Spell);
        this.SpellButton.Disable(num <= 0);
        this.SpellButtonDisabledIcon.SetActive(num <= 0);
        num = character.Deck.Filter(CardType.Item);
        this.ItemButton.Disable(num <= 0);
        this.ItemButtonDisabledIcon.SetActive(num <= 0);
        num = character.Deck.Filter(CardType.Armor);
        this.ArmorButton.Disable(num <= 0);
        this.ArmorButtonDisabledIcon.SetActive(num <= 0);
        num = character.Deck.Filter(CardType.Ally);
        this.AllyButton.Disable(num <= 0);
        this.AllyButtonDisabledIcon.SetActive(num <= 0);
        num = character.Deck.Filter(CardType.Blessing);
        this.BlessingButton.Disable(num <= 0);
        this.BlessingButtonDisabledIcon.SetActive(num <= 0);
        num = this.CountLootCards(character.Deck);
        this.LootButton.Disable(num <= 0);
        this.LootButtonDisabledIcon.SetActive(num <= 0);
    }

    private int CountLootCards(Deck deck)
    {
        int num = 0;
        for (int i = 0; i < deck.Count; i++)
        {
            if (CardTable.LookupCardType(deck[i].ID) == CardType.Loot)
            {
                num++;
            }
        }
        return num;
    }

    private void Finish()
    {
        this.Show(false);
        UI.Window.SendMessage("OnFinishButtonPushed");
    }

    private void OnCardFilterAlliesButtonPushed()
    {
        this.SelectCardType(CardType.Ally, 5);
    }

    private void OnCardFilterArmorsButtonPushed()
    {
        this.SelectCardType(CardType.Armor, 3);
    }

    private void OnCardFilterBlessingsButtonPushed()
    {
        this.SelectCardType(CardType.Blessing, 6);
    }

    private void OnCardFilterItemsButtonPushed()
    {
        this.SelectCardType(CardType.Item, 4);
    }

    private void OnCardFilterLootButtonPushed()
    {
        this.SelectCardType(CardType.Loot, 7);
    }

    private void OnCardFilterSpellsButtonPushed()
    {
        this.SelectCardType(CardType.Spell, 2);
    }

    private void OnCardFilterWeaponsButtonPushed()
    {
        this.SelectCardType(CardType.Weapon, 1);
    }

    private void SelectCardType(CardType type, int animNumber)
    {
        if (!this.isBusy)
        {
            this.isBusy = true;
            this.PanelAnimator.SetInteger("Selected_Type", animNumber);
            this.PanelAnimator.SetTrigger("Selected");
            if (this.myCharacter != null)
            {
                this.myCharacter.FavoredCard = type;
            }
            LeanTween.delayedCall(1.75f, new Action(this.Finish));
        }
    }

    public void Show(Character character)
    {
        this.myCharacter = character;
        this.Portrait.Image = this.myCharacter.PortraitAvatar;
        this.ConfigureButtonStates(character);
        this.isBusy = false;
        this.Show(true);
        this.PanelAnimator.SetTrigger("Start");
        Tutorial.Notify(TutorialEventType.ScreenChooseFavoriteCardShown);
    }
}

