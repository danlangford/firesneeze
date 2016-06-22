using System;

public class GuiLayoutHidden : GuiLayout
{
    public override bool OnGuiDrag(Card card) => 
        false;

    public override bool OnGuiDrop(Card card)
    {
        if (card != null)
        {
            card.transform.position = base.transform.position;
            card.transform.localScale = this.Scale;
            card.Show(false);
        }
        return false;
    }

    public override void Refresh()
    {
        if (this.Deck != null)
        {
            for (int i = 0; i < this.Deck.Count; i++)
            {
                this.Deck[i].transform.position = base.transform.position;
                this.Deck[i].transform.localScale = this.Scale;
                this.Deck[i].Show(false);
            }
        }
    }
}

