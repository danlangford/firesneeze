using System;
using System.Runtime.InteropServices;

[AttributeUsage(AttributeTargets.Field, AllowMultiple=false)]
public class StrRefAttr : Attribute
{
    private int id;
    private string table;

    public StrRefAttr(int id, string table = "UI")
    {
        this.id = id;
        this.table = table;
    }

    public string Text =>
        StringTableManager.Get(this.table, this.id);
}

