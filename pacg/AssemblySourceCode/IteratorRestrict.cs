using System;

public class IteratorRestrict : TurnStateIterator
{
    public override void End()
    {
        base.End();
        Location.Load(Turn.Character.Location);
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRoll_End"));
        Turn.GotoStateDestination();
    }

    public override void Invoke()
    {
        Turn.Current = Turn.Number;
        Location.Load(Turn.Character.Location);
        this.SummonTemporaryEncounterCard();
        base.RefreshLocationWindow();
        Turn.PushStateDestination(new TurnStateCallback(Turn.Card, "EventRoll_Check"));
        Turn.GotoStateDestination();
    }

    public override bool IsValid()
    {
        if ((Turn.DamageTargetType == DamageTargetType.Location) && (Location.CountCharactersAtLocation(Location.Current.ID) <= 1))
        {
            return false;
        }
        if (Party.CountLivingMembers() <= 0)
        {
            return false;
        }
        return base.IsValid();
    }

    public override bool Next()
    {
        if ((Turn.DamageTargetType == DamageTargetType.Location) && base.NextCharacterAtLocation(Location.Current.ID))
        {
            return true;
        }
        if (Turn.DamageTargetType == DamageTargetType.Party)
        {
            this.UnsummonTemporaryEncounterCard();
            if (base.NextCharacterInParty())
            {
                return true;
            }
        }
        return false;
    }

    public override void Start()
    {
        base.Start();
    }

    private void SummonTemporaryEncounterCard()
    {
        if (Location.Current.ID != base.InitialLocation)
        {
            Card card = CardTable.Create(base.InitialCard);
            if (card != null)
            {
                card.Disposition = DispositionType.Destroy;
                Location.Current.Deck.Add(card, DeckPositionType.Top);
            }
        }
    }

    private void UnsummonTemporaryEncounterCard()
    {
        if ((Location.Current.ID != base.InitialLocation) && (Location.Current.Deck.Count > 0))
        {
            Card card = Location.Current.Deck[0];
            if (card.ID == base.InitialCard)
            {
                Location.Current.Deck.Remove(card);
            }
        }
    }

    public override TurnStateIteratorType Type =>
        TurnStateIteratorType.Restrict;
}

