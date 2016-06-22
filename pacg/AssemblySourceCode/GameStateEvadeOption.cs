using System;

public class GameStateEvadeOption : GameState
{
    public override void Enter()
    {
        base.Enter();
        Turn.EvadeDeclined = false;
        Turn.SetEncounteredInformation();
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.layoutLocation.ShowPreludeFX(true);
            window.layoutLocation.Refresh();
            for (int i = 0; i < window.layoutHand.Deck.Count; i++)
            {
                if (window.layoutHand.Deck[i].Displayed)
                {
                    window.layoutHand.Deck[i].Locked = true;
                }
            }
        }
        if (this.HasBeforeEncounterEvent(Turn.Card) && this.IsEvadePossible(Turn.Card))
        {
            this.GivePlayerOptionToEvade();
            base.Message(0x5d);
        }
        else if (this.HasBeforeEncounterWait(Turn.Card))
        {
            this.GivePlayerOptionToEvade();
        }
        else if (!Turn.Evade)
        {
            this.Proceed();
        }
    }

    public override void Exit(GameStateType nextState)
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowEncounterButton(false);
            window.ShowProceedButton(false);
            window.ShowCancelButton(false);
        }
    }

    private void GivePlayerOptionToEvade()
    {
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            window.ShowEncounterButton(true);
        }
    }

    private bool HasBeforeEncounterEvent(Card card)
    {
        if ((Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponent<EventEncounteredDamageLocation>() != null) && Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponent<EventEncounteredDamageLocation>().IsEventValid(card))
        {
            return true;
        }
        if ((Location.Current.GetComponent<EventBeforeActCheck>() != null) && Scenario.Current.GetLocationPowersRoot(Location.Current.ID).GetComponent<EventBeforeActCheck>().IsEventValid(card))
        {
            return true;
        }
        for (int i = 0; i < Scenario.Current.Powers.Count; i++)
        {
            EventEncounteredRollBlock component = Scenario.Current.Powers[i].GetComponent<EventEncounteredRollBlock>();
            if ((component != null) && component.IsEventValid(card))
            {
                return true;
            }
        }
        Event[] components = card.GetComponents<Event>();
        for (int j = 0; (components != null) && (j < components.Length); j++)
        {
            if (components[j].IsEventImplemented(EventType.OnCardBeforeAct))
            {
                return true;
            }
        }
        return Rules.IsTemporaryClosePossible();
    }

    private bool HasBeforeEncounterWait(Card card) => 
        (card.GetComponent<EventEncounteredWait>() != null);

    private bool HasStatelessBeforeEncounterEvent(Card card)
    {
        Event[] components = card.GetComponents<Event>();
        for (int i = 0; i < components.Length; i++)
        {
            if ((components[i].Type == EventType.OnCardEncountered) && !components[i].Stateless)
            {
                return false;
            }
        }
        return true;
    }

    public override bool IsActionAllowed(ActionType action, Card card) => 
        card.IsActionAllowed(action);

    private bool IsEvadePossible(Card encounter)
    {
        if (encounter.GetComponent<CardPropertyCannotEvade>() == null)
        {
            for (int i = 0; i < Turn.Character.Powers.Count; i++)
            {
                if ((Turn.Character.Powers[i].Type == PowerType.Evade) && Turn.Character.Powers[i].IsValid())
                {
                    return true;
                }
            }
            for (int j = 0; j < Turn.Character.Hand.Count; j++)
            {
                Card card = Turn.Character.Hand[j];
                CardPower[] components = card.GetComponents<CardPower>();
                for (int n = 0; n < components.Length; n++)
                {
                    if ((components[n].Type == PowerType.Evade) && components[n].IsValid(card))
                    {
                        return true;
                    }
                }
            }
            for (int k = 0; k < Scenario.Current.Powers.Count; k++)
            {
                if ((Scenario.Current.Powers[k].Type == PowerType.Evade) && Scenario.Current.Powers[k].IsValid())
                {
                    return true;
                }
            }
            for (int m = 0; m < Location.Current.Powers.Count; m++)
            {
                if ((Location.Current.Powers[m].Type == PowerType.Evade) && Location.Current.Powers[m].IsValid())
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsOnlyStatelessEncounterEvents(Card card)
    {
        Event[] components = card.GetComponents<Event>();
        for (int i = 0; i < components.Length; i++)
        {
            if ((components[i].IsEventImplemented(EventType.OnCardBeforeAct) && !components[i].Stateless) && components[i].IsEventValid(card))
            {
                return false;
            }
        }
        return true;
    }

    public override void Proceed()
    {
        if (this.HasBeforeEncounterEvent(Turn.Card))
        {
            base.Message((string) null);
            if (!Turn.Evade)
            {
                Turn.EvadeDeclined = true;
            }
        }
        if (Turn.Evade)
        {
            Party.OnCheckCompleted();
            if (base.IsCurrentState())
            {
                Turn.State = GameStateType.Post;
            }
        }
        else
        {
            Turn.Card.OnBeforeAct();
            if (base.IsCurrentState() && this.IsOnlyStatelessEncounterEvents(Turn.Card))
            {
                Turn.State = GameStateType.Combat;
            }
            Tutorial.Notify(TutorialEventType.CardEncountered);
        }
    }

    public override GameStateType Type =>
        GameStateType.EvadeOption;
}

