using System;
using System.Collections.Generic;

public class Collection
{
    private static bool isLoaded = false;
    private static Dictionary<string, CollectionEntry> table = new Dictionary<string, CollectionEntry>();

    public static bool Add(string id) => 
        Add(id, 1);

    public static bool Add(string id, int quantity)
    {
        if (!Game.Network.Connected || Game.Network.OutOfDate)
        {
            return false;
        }
        Game.Network.AddCardToCollection(id, quantity);
        for (int i = 0; i < quantity; i++)
        {
            Push(id);
        }
        return true;
    }

    public static void Clear()
    {
        table.Clear();
        isLoaded = false;
    }

    public static bool Contains(string id) => 
        table.ContainsKey(id);

    public static List<string> GetCards(string set)
    {
        List<string> list = new List<string>(Count);
        foreach (CollectionEntry entry in table.Values)
        {
            if (string.IsNullOrEmpty(set) || (entry.set == set))
            {
                for (int i = 0; i < entry.quantity; i++)
                {
                    list.Add(entry.id);
                }
            }
        }
        return list;
    }

    public static int GetCost(string id)
    {
        CollectionEntry entry;
        if (table.TryGetValue(id, out entry))
        {
            return entry.cost;
        }
        return 0;
    }

    public static List<CollectionEntry> GetEntries(string set)
    {
        List<CollectionEntry> list = new List<CollectionEntry>(Count);
        foreach (CollectionEntry entry in table.Values)
        {
            if (string.IsNullOrEmpty(set) || (entry.set == set))
            {
                list.Add(entry);
            }
        }
        return list;
    }

    public static void Load(List<CollectionEntry> data)
    {
        Clear();
        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                table.Add(data[i].id, data[i]);
            }
        }
        isLoaded = true;
    }

    public static bool Pull(string id)
    {
        CollectionEntry entry;
        if (!table.TryGetValue(id, out entry))
        {
            return false;
        }
        entry.quantity--;
        if (entry.quantity <= 0)
        {
            table.Remove(id);
        }
        return true;
    }

    public static void Push(string[] cards)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            Push(cards[i]);
        }
    }

    public static void Push(string id)
    {
        CollectionEntry entry;
        if (table.TryGetValue(id, out entry))
        {
            entry.quantity++;
        }
        else
        {
            entry = new CollectionEntry(id);
            table.Add(id, entry);
        }
    }

    public static bool Remove(string id) => 
        Remove(id, 1);

    public static bool Remove(string id, int quantity)
    {
        CollectionEntry entry;
        if (!table.TryGetValue(id, out entry))
        {
            return false;
        }
        Game.Network.RemoveCardFromCollection(id, quantity);
        entry.quantity -= quantity;
        if (entry.quantity <= 0)
        {
            table.Remove(id);
        }
        return true;
    }

    public static List<string> Cards =>
        GetCards(null);

    public static int Count
    {
        get
        {
            int num = 0;
            foreach (CollectionEntry entry in table.Values)
            {
                num += entry.quantity;
            }
            return num;
        }
    }

    public static List<CollectionEntry> Entries =>
        GetEntries(null);

    public static bool Loaded =>
        isLoaded;
}

