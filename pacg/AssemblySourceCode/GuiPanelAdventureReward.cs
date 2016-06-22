using System;
using UnityEngine;

public class GuiPanelAdventureReward : GuiPanelBackStack
{
    [Tooltip("spawn location for the card")]
    public GameObject CardHolder;

    public override void Clear()
    {
        Geometry.DestroyAllChildren(this.CardHolder);
    }

    private string GetAdventurePrize(Adventure adventure)
    {
        if (adventure != null)
        {
            PrizeTreasureCard component = adventure.GetComponent<PrizeTreasureCard>();
            if (component != null)
            {
                return component.PrizeCard;
            }
        }
        return null;
    }

    public override void Initialize()
    {
        base.Initialize();
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        this.Show(false);
    }

    public void Show(Adventure adventure)
    {
        string adventurePrize = this.GetAdventurePrize(adventure);
        if (!string.IsNullOrEmpty(adventurePrize))
        {
            base.Show(true);
            Card card = CardTable.Create(adventurePrize);
            if (card != null)
            {
                card.SortingLayer = "UI";
                card.SortingOrder = 2;
                card.Show(CardSideType.Front);
                Geometry.AddChild(this.CardHolder, card.gameObject);
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        if (!isVisible)
        {
            this.Clear();
        }
    }

    public override uint zIndex =>
        Constants.ZINDEX_PANEL_POPUP;
}

