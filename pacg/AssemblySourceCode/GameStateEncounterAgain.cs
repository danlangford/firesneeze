using System;

public class GameStateEncounterAgain : GameState
{
    private void EnqueueEncounterEvents()
    {
        Event[] components;
        Turn.Card.OnSecondCombat(Turn.Card);
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            components = Scenario.Current.Powers[i].GetComponents<Event>();
            if (components != null)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j].IsEventImplemented(EventType.OnCardEncountered) && components[j].TriggerOnEachCheck)
                    {
                        Game.Events.Add(EventCallbackType.Scenario, components[j].name, EventType.OnCardEncountered, j, components[j].Stateless, Turn.Card);
                    }
                }
            }
        }
        components = Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponents<Event>();
        if (components != null)
        {
            for (int k = 0; k < components.Length; k++)
            {
                if (components[k].IsEventImplemented(EventType.OnCardEncountered) && components[k].TriggerOnEachCheck)
                {
                    Game.Events.Add(EventCallbackType.Location, Location.Current.ID, EventType.OnCardEncountered, k, components[k].Stateless, Turn.Card);
                }
            }
        }
        components = Turn.Card.GetComponents<Event>();
        if (components != null)
        {
            for (int m = 0; m < components.Length; m++)
            {
                if (components[m].IsEventImplemented(EventType.OnCardEncountered) && components[m].TriggerOnEachCheck)
                {
                    Game.Events.Add(EventCallbackType.Card, Turn.Card.ID, EventType.OnCardEncountered, m, components[m].Stateless, Turn.Card);
                }
                if (components[m].IsEventImplemented(EventType.OnCardBeforeAct) && components[m].TriggerOnEachCheck)
                {
                    Game.Events.Add(EventCallbackType.Card, Turn.Card.ID, EventType.OnCardBeforeAct, m, components[m].Stateless, Turn.Card);
                }
            }
        }
    }

    public override void Enter()
    {
        this.EnqueueEncounterEvents();
        if (Turn.State == GameStateType.EncounterAgain)
        {
            Turn.Proceed();
        }
    }

    public override void Proceed()
    {
        Turn.Defeat = false;
        Turn.PushStateDestination(GameStateType.Combat);
        Turn.State = GameStateType.Recharge;
    }

    protected override bool Persistent =>
        false;

    public override GameStateType Type =>
        GameStateType.EncounterAgain;
}

