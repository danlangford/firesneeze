using System;
using UnityEngine;

public class GuiLayoutSingle : GuiLayout
{
    private bool isVisible;

    public void Display(int i)
    {
        this.isVisible = true;
        if (this.Deck.Count > i)
        {
            Card card = this.Deck[i];
            card.transform.localScale = this.Scale;
            card.transform.position = base.transform.position + new Vector3(0f, card.Size.y, 0f);
            card.SortingOrder = 1;
            card.Show(true);
            Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_IGNORE);
            Vector3 vector = new Vector3(base.transform.position.x - 3f, base.transform.position.y, base.transform.position.z);
            Vector3[] destinations = new Vector3[] { card.transform.position, vector, vector, base.transform.position };
            card.MoveCard(destinations, 0.4f).setOrientToPath(false).setEase(LeanTweenType.easeOutQuad);
        }
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                this.Deck[i].Show(false);
            }
            if (this.isVisible && (this.Deck.Count > 0))
            {
                Card card = this.Deck[0];
                card.transform.localScale = this.Scale;
                card.transform.position = base.transform.position;
                card.SortingOrder = 1;
                card.Show(true);
                Geometry.SetLayerRecursively(card.gameObject, Constants.LAYER_IGNORE);
            }
        }
    }
}

