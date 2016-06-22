using System;
using UnityEngine;

public class GuiPanelStorePreview : GuiPanel
{
    private Card[] cards;
    [Tooltip("references to spawnpoints (and scale) of cards to appear")]
    public GameObject[] CardSpawnPoints;
    private readonly int NUM_PREVIEW_CARDS = 3;
    private TKTapRecognizer tapRecognizer;
    private Card zoomedCard;

    private void DisplayCards(string id, int position)
    {
        Card card = CardTable.Create(id);
        if (card != null)
        {
            card.Animations(false);
            card.transform.position = this.CardSpawnPoints[position].transform.position;
            card.transform.localScale = this.CardSpawnPoints[position].transform.localScale;
            card.SortingOrder = Constants.SPRITE_SORTING_ZOOM - 1;
            card.SortingLayer = "UI";
            card.Show(true);
            this.cards[position] = card;
        }
    }

    public override void Initialize()
    {
        this.cards = new Card[this.NUM_PREVIEW_CARDS];
        this.tapRecognizer = new TKTapRecognizer();
        this.tapRecognizer.zIndex = 3;
        this.tapRecognizer.gestureRecognizedEvent += r => this.OnGuiTap(this.tapRecognizer.touchLocation());
        TouchKit.addGestureRecognizer(this.tapRecognizer);
        this.tapRecognizer.enabled = false;
        this.Show(false);
    }

    private void OnCloseButtonPushed()
    {
        this.zoomedCard = null;
        UI.Busy = false;
        this.Show(false);
    }

    private void OnGuiTap(Vector2 touchPos)
    {
        if (this.zoomedCard != null)
        {
            this.zoomedCard.OnGuiZoom(false);
            this.zoomedCard = null;
        }
        else
        {
            Card topCard = GuiLayout.GetTopCard(touchPos);
            if (topCard != null)
            {
                this.zoomedCard = topCard;
                topCard.OnGuiZoom(true);
            }
        }
    }

    public override void Show(bool isVisible)
    {
        base.Show(isVisible);
        this.tapRecognizer.enabled = isVisible;
        if (!isVisible)
        {
            for (int i = 0; i < this.cards.Length; i++)
            {
                if (this.cards[i] != null)
                {
                    UnityEngine.Object.Destroy(this.cards[i].gameObject);
                }
            }
        }
    }

    public void Show(string productCode)
    {
        LicenseTableEntry entry = LicenseTable.Get(productCode);
        if (entry != null)
        {
            this.Show(true);
            if (entry.Type == LicenseType.Adventure)
            {
                for (int i = 0; i < entry.Preview.Length; i++)
                {
                    this.DisplayCards(entry.Preview[i], i);
                }
            }
        }
    }
}

