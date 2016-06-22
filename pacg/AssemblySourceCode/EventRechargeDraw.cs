using System;

public class EventRechargeDraw : Event
{
    public override void OnCardRecharged(Card card)
    {
        if ((Turn.RechargeReason == GameReasonType.MonsterForced) && (Turn.Character.Deck.Count > 0))
        {
            Card card2 = Turn.Character.Deck.Draw();
            Turn.Character.Hand.Add(card2);
            GuiWindowLocation window = UI.Window as GuiWindowLocation;
            if (window != null)
            {
                window.layoutHand.Refresh();
            }
        }
    }

    public override EventType Type =>
        EventType.OnCardRecharged;
}

