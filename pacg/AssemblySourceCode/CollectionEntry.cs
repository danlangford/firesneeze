using System;

public class CollectionEntry
{
    public int cost;
    public string id;
    public int quantity;
    public string set;

    public CollectionEntry(string id)
    {
        this.id = id;
        this.set = CardTable.LookupCardSet(id);
        this.quantity = 1;
        this.cost = 0;
    }
}

