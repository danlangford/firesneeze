using System;
using System.Collections.Generic;

public class EventUndefeatedVillain : Event
{
    public override void OnCardUndefeated(Card card)
    {
        if (Rules.IsCardSummons(card))
        {
            Event.Done();
        }
        else
        {
            List<string> list = new List<string>(Scenario.Current.Locations.Length);
            for (int i = 0; i < Scenario.Current.Locations.Length; i++)
            {
                if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName))
                {
                    string locationName = Scenario.Current.Locations[i].LocationName;
                    if (!Scenario.Current.IsLocationClosed(locationName))
                    {
                        list.Add(locationName);
                    }
                }
            }
            list.Shuffle<string>();
            if (list.Count > 0)
            {
                string locID = list[0];
                if (locID != Location.Current.ID)
                {
                    Location.Distribute(locID, card, DeckPositionType.Shuffle, false);
                    card.Disposition = DispositionType.Destroy;
                }
                else
                {
                    card.Disposition = DispositionType.Shuffle;
                }
            }
            for (int j = 1; j < list.Count; j++)
            {
                Card card2 = Scenario.Current.Blessings.Draw();
                if (card2 != null)
                {
                    string str3 = list[j];
                    if (str3 != Location.Current.ID)
                    {
                        Location.Distribute(str3, card2, DeckPositionType.Shuffle, false);
                        card2.Destroy();
                    }
                    else
                    {
                        Location.Current.Deck.Add(card2, DeckPositionType.Bottom);
                        Location.Current.Deck.ShuffleUnderTop();
                        Scenario.Current.AddCardCount(Location.Current.ID, CardType.Blessing, -1);
                        Scenario.Current.AddCardCount(Location.Current.ID, CardType.None, 1);
                    }
                }
                else
                {
                    Turn.State = GameStateType.End;
                    return;
                }
            }
            (UI.Window as GuiWindowLocation).locationPanel.RefreshCardList();
            card.Show(false);
            Turn.PushStateDestination(GameStateType.Flee);
            Turn.State = GameStateType.Recharge;
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

