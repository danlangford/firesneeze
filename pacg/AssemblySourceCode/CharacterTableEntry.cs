using System;

public class CharacterTableEntry
{
    public int descriptionStrRef;
    public int nameStrRef;
    public string set;

    public string Description =>
        StringTableManager.Get(CharacterTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(CharacterTable.Name, this.nameStrRef);
}

