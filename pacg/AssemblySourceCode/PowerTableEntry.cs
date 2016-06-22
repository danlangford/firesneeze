using System;

public class PowerTableEntry
{
    public bool Active;
    public int descriptionStrRef;
    public string Family;
    public string Icon;
    public int Index;
    public bool Modifier;
    public int nameStrRef;
    public int Rank;
    public string Requires;

    public string GetIconPath(string id) => 
        ("Blueprints/Icons/Powers/" + id);

    public string Description =>
        StringTableManager.Get(PowerTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(PowerTable.Name, this.nameStrRef);
}

