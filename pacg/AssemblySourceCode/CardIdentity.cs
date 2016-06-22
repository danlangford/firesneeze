using System;

public class CardIdentity
{
    public string ID;
    public string Set;

    public CardIdentity(Card card)
    {
        this.ID = card.ID;
        this.Set = card.Set;
    }

    public CardIdentity(string id)
    {
        this.ID = id;
        CardTableEntry entry = CardTable.Get(this.ID);
        if (entry != null)
        {
            this.Set = entry.set;
        }
    }

    public CardIdentity(string id, string set)
    {
        this.ID = id;
        this.Set = set;
    }
}

