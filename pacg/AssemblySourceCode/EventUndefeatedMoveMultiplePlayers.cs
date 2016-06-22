using System;
using UnityEngine;

public class EventUndefeatedMoveMultiplePlayers : Event
{
    [Tooltip("selector that determines where to move the players")]
    public LocationSelector LocationSelector;
    [Tooltip("who to move")]
    public DamageTargetType Target;

    public override void OnCardUndefeated(Card card)
    {
        switch (this.Target)
        {
            case DamageTargetType.Player:
                if (Turn.Character.Alive)
                {
                    Turn.BlackBoard.Set<string>(Turn.Character.ID + "_destination", this.LocationSelector.Random(Turn.Character));
                }
                break;

            case DamageTargetType.Location:
                for (int i = 0; i < Party.Characters.Count; i++)
                {
                    if ((Party.Characters[i].Location == Location.Current.ID) && Party.Characters[i].Alive)
                    {
                        Turn.BlackBoard.Set<string>(Party.Characters[i].ID + "_destination", this.LocationSelector.Random(Party.Characters[i]));
                    }
                }
                break;

            case DamageTargetType.Party:
                for (int j = 0; j < Party.Characters.Count; j++)
                {
                    if (Party.Characters[j].Alive)
                    {
                        Turn.BlackBoard.Set<string>(Party.Characters[j].ID + "_destination", this.LocationSelector.Random(Party.Characters[j]));
                    }
                }
                break;
        }
        Turn.DamageTargetType = this.Target;
        Turn.Iterators.Start(TurnStateIteratorType.Move);
        Turn.Iterators.Invoke();
    }

    public override EventType Type =>
        EventType.OnCardUndefeated;
}

