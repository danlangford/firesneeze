using System;

public class EffectHoldCard : Effect
{
    public EffectHoldCard(string source, int duration) : base(source, duration)
    {
    }

    public override string GetDisplayText() => 
        "Medusa Mask";

    public override void Invoke()
    {
        if (Location.Current.Deck.Count > 0)
        {
            Card card = Location.Current.Deck[0];
            base.source = card.ID;
            Location.Current.Deck.Remove(card);
            card.Destroy();
        }
    }

    public override void OnEffectFinished()
    {
        Card card = CardTable.Create(base.source);
        if (card != null)
        {
            Location.Current.Deck.Add(card, DeckPositionType.Top);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.locationPanel.RefreshCardList();
                window.layoutExplore.Refresh();
                window.layoutExplore.Display();
            }
        }
    }

    public override EffectType Type =>
        EffectType.HoldCard;
}

