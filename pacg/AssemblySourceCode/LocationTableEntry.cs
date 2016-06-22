using System;

public class LocationTableEntry
{
    public int closedStrRef;
    public int closingStrRef;
    public string deck;
    public int locationStrRef;
    public int nameStrRef;
    private static readonly char[] seperator = new char[] { ',' };
    public string set;

    public int GetCardCount(CardType type)
    {
        if (this.deck != null)
        {
            string[] strArray = this.deck.Split(seperator);
            if ((strArray != null) && (strArray.Length >= 7))
            {
                if (type == CardType.Monster)
                {
                    return int.Parse(strArray[0]);
                }
                if (type == CardType.Barrier)
                {
                    return int.Parse(strArray[1]);
                }
                if (type == CardType.Weapon)
                {
                    return int.Parse(strArray[2]);
                }
                if (type == CardType.Spell)
                {
                    return int.Parse(strArray[3]);
                }
                if (type == CardType.Armor)
                {
                    return int.Parse(strArray[4]);
                }
                if (type == CardType.Item)
                {
                    return int.Parse(strArray[5]);
                }
                if (type == CardType.Ally)
                {
                    return int.Parse(strArray[6]);
                }
                if (type == CardType.Blessing)
                {
                    return int.Parse(strArray[7]);
                }
            }
        }
        return 0;
    }

    public string Closed =>
        StringTableManager.Get(LocationTable.Name, this.closedStrRef);

    public string Closing =>
        StringTableManager.Get(LocationTable.Name, this.closingStrRef);

    public string Location =>
        StringTableManager.Get(LocationTable.Name, this.locationStrRef);

    public string Name =>
        StringTableManager.Get(LocationTable.Name, this.nameStrRef);
}

