using System;

public class LicenseTableEntry
{
    public string Art;
    public bool Available;
    public string Date;
    public int descriptionStrRef;
    public string Icon;
    public int nameStrRef;
    public string Nickname;
    public string[] Preview;
    public LicenseType Type;

    public string Description =>
        StringTableManager.Get(LicenseTable.Name, this.descriptionStrRef);

    public string Name =>
        StringTableManager.Get(LicenseTable.Name, this.nameStrRef);
}

