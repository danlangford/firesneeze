using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct StringTableReference
{
    [Tooltip("a string table file name")]
    public string file;
    [Tooltip("a string id within the file")]
    public int id;
    private static readonly string[] SEPARATOR;
    public StringTableReference(string file, int id)
    {
        this.file = file;
        this.id = id;
    }

    static StringTableReference()
    {
        SEPARATOR = new string[] { "/" };
    }

    public bool IsValid() => 
        (this.file != null);

    public static StringTableReference Create(string s)
    {
        string[] strArray = s.Split(SEPARATOR, StringSplitOptions.RemoveEmptyEntries);
        if ((strArray != null) && (strArray.Length == 2))
        {
            return new StringTableReference(strArray[0], int.Parse(strArray[1]));
        }
        return new StringTableReference(null, 0);
    }
}

