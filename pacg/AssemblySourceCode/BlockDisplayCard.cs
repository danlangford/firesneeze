using System;
using UnityEngine;

public class BlockDisplayCard : Block
{
    [Tooltip("the card to be displayed in the summons tray")]
    public string ID;
    [Tooltip("only cards that match will be displayed")]
    public CardSelector Selector;

    public override void Invoke()
    {
        if ((Location.Current.Deck.Count > 0) && ((this.Selector == null) || this.Selector.Match(Location.Current.Deck[0])))
        {
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutSummoner.Show(this.ID);
            }
        }
    }
}

