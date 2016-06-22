using System;

[Serializable]
public class StrRefType
{
    public string file = "None";
    public int id;

    public bool IsNullOrEmpty() => 
        (string.IsNullOrEmpty(this.file) || (this.file == "None"));

    public override string ToString() => 
        StringTableManager.Get(this);
}

