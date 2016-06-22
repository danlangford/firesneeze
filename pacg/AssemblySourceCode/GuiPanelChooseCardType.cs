using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GuiPanelChooseCardType : GuiPanel
{
    public GuiGrid Grid;
    [Tooltip("all the buttons in the correct order relative to the number the CardType is. One of these is empty because it's the villain button.")]
    public GuiButton[] TypeButtons;

    private void OnCardFilterAlliesButtonPushed()
    {
        this.SelectCardType(CardType.Ally, 5);
    }

    private void OnCardFilterArmorsButtonPushed()
    {
        this.SelectCardType(CardType.Armor, 3);
    }

    private void OnCardFilterBarriersButtonPushed()
    {
        this.SelectCardType(CardType.Barrier, 7);
    }

    private void OnCardFilterBlessingsButtonPushed()
    {
        this.SelectCardType(CardType.Blessing, 6);
    }

    private void OnCardFilterItemsButtonPushed()
    {
        this.SelectCardType(CardType.Item, 4);
    }

    private void OnCardFilterMonstersButtonPushed()
    {
        this.SelectCardType(CardType.Monster, 8);
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
        if (!UI.Busy)
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.UnZoomCard();
            }
            this.SelectedCardType = type;
            Animator componentInChildren = base.GetComponentInChildren<Animator>();
            if (componentInChildren != null)
            {
                componentInChildren.SetInteger("Selected_Type", animNumber);
            }
            UI.Busy = true;
            LeanTween.delayedCall(0.25f, new Action(this.SelectCardTypeFinish));
        }
    }

    private void SelectCardTypeFinish()
    {
        UI.Busy = false;
        Turn.Proceed();
    }

    public void Show(CardFilter filter)
    {
        base.Show(true);
        for (int i = 0; i < this.TypeButtons.Length; i++)
        {
            if (this.TypeButtons[i] != null)
            {
                this.TypeButtons[i].gameObject.SetActive(false);
                this.TypeButtons[i].Refresh();
            }
        }
        for (int j = 0; j < filter.CardTypes.Length; j++)
        {
            this.TypeButtons[((int) filter.CardTypes[j]) - 1].gameObject.SetActive(true);
            this.TypeButtons[((int) filter.CardTypes[j]) - 1].Refresh();
        }
        this.Grid.Show(true);
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (isVisible)
        {
            for (int i = 0; i < this.TypeButtons.Length; i++)
            {
                if (this.TypeButtons[i] != null)
                {
                    this.TypeButtons[i].gameObject.SetActive(true);
                    this.TypeButtons[i].Refresh();
                }
            }
            this.Grid.Show(isVisible);
        }
    }

    public CardType SelectedCardType { get; private set; }
}

