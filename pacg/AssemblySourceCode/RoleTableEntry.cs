using System;

public class RoleTableEntry
{
    public int descriptionStrRef;
    public int HandSize;
    public int nameStrRef;
    public string[] Powers;
    public ProficencyType[] Proficiencies;

    public override bool Equals(object obj) => 
        ((obj is RoleTableEntry) && (((RoleTableEntry) obj).nameStrRef == this.nameStrRef));

    public override int GetHashCode() => 
        this.nameStrRef.GetHashCode();

    public string Description =>
        StringTableManager.Get(RoleTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(RoleTable.Name, this.nameStrRef);
}

