using System;

public class AdventureTableEntry
{
    public int descriptionStrRef;
    public string id;
    public int nameStrRef;
    public int rewardStrRef;
    public string set;

    public string Description =>
        StringTableManager.Get(AdventureTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(AdventureTable.Name, this.nameStrRef);

    public string Reward =>
        StringTableManager.Get(AdventureTable.Name, this.rewardStrRef);
}

