using System;

public class ScenarioPowerTableEntry
{
    public int descriptionStrRef;
    public string id;
    public int nameStrRef;

    public string Description =>
        StringTableManager.Get(ScenarioPowerTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(ScenarioPowerTable.Name, this.nameStrRef);
}

