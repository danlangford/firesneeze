using System;
using System.Collections.Generic;

public class EventDefeatedVillain : Event
{
    public override void EndGameIfNecessary(Card card)
    {
        if (this.IsEarlyEndGameNecessary(card))
        {
            Event[] components = base.GetComponents<Event>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].IsEventImplemented(EventType.OnCombatResolved))
                {
                    return;
                }
            }
            this.OnCardDefeated(card);
        }
    }

    public virtual bool IsAnimationPossible(AnimationType animation) => 
        ((animation != AnimationType.Defeated) && (animation != AnimationType.Undefeated));

    protected virtual bool IsEarlyEndGameNecessary(Card card)
    {
        if (!Rules.IsEncounterInCurrentLocation())
        {
            return false;
        }
        if (Rules.IsCardSummons(card))
        {
            return false;
        }
        return (((this.IsEventValid(card) && (Scenario.Current.GetNumOpenLocations() <= 1)) && !Game.Events.ContainsStatefulEvent()) && !Location.Current.StopsVillainCorner);
    }

    public override bool IsEventValid(Card card)
    {
        if (Turn.LastCombatResult != CombatResultType.Win)
        {
            return false;
        }
        if (!card.OnCombatEnd())
        {
            return false;
        }
        return base.IsEventValid(card);
    }

    public override void OnCardDefeated(Card card)
    {
        if (Rules.IsCardSummons(card))
        {
            Event.Done();
        }
        else
        {
            if (Rules.IsEncounterInCurrentLocation())
            {
                Turn.CloseType = CloseType.Villain;
                Turn.DamageTargetType = DamageTargetType.None;
            }
            List<string> list = new List<string>(Scenario.Current.Locations.Length);
            for (int i = 0; i < Scenario.Current.Locations.Length; i++)
            {
                if (Scenario.Current.IsLocationValid(Scenario.Current.Locations[i].LocationName))
                {
                    string locationName = Scenario.Current.Locations[i].LocationName;
                    if (!Scenario.Current.IsLocationClosed(locationName) && (!Rules.IsEncounterInCurrentLocation() || (locationName != Location.Current.ID)))
                    {
                        list.Add(locationName);
                    }
                }
            }
            list.Shuffle<string>();
            if (list.Count <= 0)
            {
                Scenario.Current.Complete = true;
                Turn.State = GameStateType.PreEnd;
            }
            else
            {
                if (list.Count > 0)
                {
                    string locID = list[0];
                    Location.Distribute(locID, card, DeckPositionType.Shuffle, false);
                    card.Disposition = DispositionType.Destroy;
                }
                for (int j = 1; j < list.Count; j++)
                {
                    Card card2 = Campaign.Box.Draw(CardType.Blessing);
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
                }
                (UI.Window as GuiWindowLocation).locationPanel.RefreshCardList();
                card.Show(false);
                (UI.Window as GuiWindowLocation).layoutLocation.GlowText(false);
                Turn.State = GameStateType.Flee;
            }
        }
    }

    public override bool Stateless =>
        false;

    public override EventType Type =>
        EventType.OnCardDefeated;
}

