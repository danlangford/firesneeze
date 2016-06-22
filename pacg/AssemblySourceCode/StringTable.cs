using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;

public class StringTable
{
    public StringTable()
    {
        this.Entries = new List<StringTableEntry>();
    }

    public void Clear()
    {
        this.Entries.Clear();
    }

    public int Find(string s)
    {
        for (int i = 0; i < this.Entries.Count; i++)
        {
            if (this.Entries[i].DefaultText == s)
            {
                return i;
            }
        }
        return -1;
    }

    public string Get(int stringID) => 
        this.Get(stringID, GenderType.Male);

    public string Get(int stringID, GenderType gender)
    {
        <Get>c__AnonStorey121 storey = new <Get>c__AnonStorey121 {
            stringID = stringID
        };
        StringTableEntry entry = this.Entries.Find(new Predicate<StringTableEntry>(storey.<>m__F5));
        if (entry == null)
        {
            return string.Empty;
        }
        if (gender == GenderType.Male)
        {
            return entry.DefaultText;
        }
        if (string.IsNullOrEmpty(entry.FemaleText))
        {
            return entry.DefaultText;
        }
        return entry.FemaleText;
    }

    public static StringTable Load(string folder, string filename)
    {
        StringTable table = new StringTable();
        TextAsset asset = (TextAsset) Resources.Load(folder + "/" + filename, typeof(TextAsset));
        if (asset != null)
        {
            try
            {
                using (StringReader reader = new StringReader(asset.text))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);
                    XmlElement documentElement = document.DocumentElement;
                    if (documentElement != null)
                    {
                        IEnumerator enumerator = documentElement.ChildNodes.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                XmlNode current = (XmlNode) enumerator.Current;
                                if ((current.Name == "EntryCount") && (table.Entries.Count == 0))
                                {
                                    int result = 0;
                                    if (int.TryParse(current.InnerText, out result) && (result > 0))
                                    {
                                        table.Entries = new List<StringTableEntry>(result);
                                    }
                                }
                                else if (current.Name == "Entries")
                                {
                                    IEnumerator enumerator2 = current.ChildNodes.GetEnumerator();
                                    try
                                    {
                                        while (enumerator2.MoveNext())
                                        {
                                            XmlNode parentNode = (XmlNode) enumerator2.Current;
                                            if (parentNode.Name == "Entry")
                                            {
                                                StringTableEntry item = new StringTableEntry(parentNode);
                                                table.Entries.Add(item);
                                            }
                                        }
                                        continue;
                                    }
                                    finally
                                    {
                                        IDisposable disposable = enumerator2 as IDisposable;
                                        if (disposable == null)
                                        {
                                        }
                                        disposable.Dispose();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable2 = enumerator as IDisposable;
                            if (disposable2 == null)
                            {
                            }
                            disposable2.Dispose();
                        }
                    }
                    return table;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(string.Concat(new object[] { "Failed: in filename: ", filename, " in folder: ", folder, " exception\n:", exception }));
            }
        }
        return table;
    }

    public static int StringToInt(string s)
    {
        int num = 0;
        for (int i = 0; i < s.Length; i++)
        {
            num = (num * 10) + (s[i] - '0');
        }
        return num;
    }

    public int Count =>
        this.Entries.Count;

    public List<StringTableEntry> Entries { get; private set; }

    [CompilerGenerated]
    private sealed class <Get>c__AnonStorey121
    {
        internal int stringID;

        internal bool <>m__F5(StringTableEntry s) => 
            (s.StringID == this.stringID);
    }
}

