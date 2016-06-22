using System;

public class AdventurePathTableEntry
{
    public int descriptionStrRef;
    public string id;
    public int nameStrRef;
    public int notesStrRef;
    public int rewardStrRef;
    public string set;

    public string Description =>
        StringTableManager.Get(AdventurePathTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(AdventurePathTable.Name, this.nameStrRef);

    public string Notes =>
        StringTableManager.Get(AdventurePathTable.Name, this.notesStrRef);

    public string Reward =>
        StringTableManager.Get(AdventurePathTable.Name, this.rewardStrRef);
}

