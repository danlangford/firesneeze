using System;

public class GuiLayoutPartyLine : GuiLayout
{
    private void GiveCard(Card card, Character character)
    {
        character.Deck.Add(card);
        card.Show(true);
        card.MoveCard(base.transform.position, 0.1f);
        LeanTween.scale(card.gameObject, this.Scale, 0.2f).setOnComplete(new Action<object>(this.GiveCardFinish)).setOnCompleteParam(card);
    }

    private void GiveCardFinish(object card)
    {
        Card card2 = card as Card;
        if (card2 != null)
        {
            card2.Show(false);
        }
    }

    public override bool OnGuiDrag(Card card) => 
        false;

    public override bool OnGuiDrop(Card card)
    {
        Character character = Party.Find(this.ID);
        if (character != null)
        {
            this.GiveCard(card, character);
            return true;
        }
        return false;
    }

    public string ID =>
        base.name;
}

