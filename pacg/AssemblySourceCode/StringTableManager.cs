using System;
using System.Collections.Generic;

public class StringTableManager
{
    private static Dictionary<string, StringTable> map = new Dictionary<string, StringTable>(5);

    public static void Clear()
    {
        map.Clear();
    }

    public static string Get(StrRefType strRef)
    {
        if (strRef != null)
        {
            return Get(strRef.file, strRef.id);
        }
        return null;
    }

    public static string Get(string id)
    {
        StringTableReference reference = StringTableReference.Create(id);
        if (!reference.IsValid())
        {
            return id;
        }
        return Get(reference.file, reference.id);
    }

    public static string Get(string tablename, int id)
    {
        if (!string.IsNullOrEmpty(tablename))
        {
            if (map.ContainsKey(tablename))
            {
                return map[tablename].Get(id);
            }
            StringTable table = LoadStringTable(tablename);
            if (table != null)
            {
                return table.Get(id);
            }
        }
        return null;
    }

    public static string GetHelperText(int id) => 
        Get("Helpertext", id);

    public static string GetUIText(int id) => 
        Get("UI", id);

    public static void Load(string tablename)
    {
        LoadStringTable(tablename);
    }

    private static StringTable LoadStringTable(string filename)
    {
        if (!string.IsNullOrEmpty(filename) && !map.ContainsKey(filename))
        {
            StringTable table = StringTable.Load("Localized/En/Text/game", filename);
            if (table != null)
            {
                map.Add(filename, table);
                return table;
            }
        }
        return null;
    }
}

