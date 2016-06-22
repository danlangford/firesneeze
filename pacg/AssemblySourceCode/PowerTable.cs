using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class PowerTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, PowerTableEntry> table = new Dictionary<string, PowerTableEntry>();

    public static PowerTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<PowerTableEntry>(i);
        }
        return null;
    }

    public static PowerTableEntry Get(string ID)
    {
        if (table.ContainsKey(ID))
        {
            return table[ID];
        }
        return null;
    }

    public static void Load()
    {
        if (!isLoaded)
        {
            StringTableManager.Load(Name);
            TextAsset asset = (TextAsset) Resources.Load("Tables/PowerTable", typeof(TextAsset));
            if (asset != null)
            {
                StringReader txtReader = new StringReader(asset.text);
                XmlDocument document = new XmlDocument();
                document.Load(txtReader);
                txtReader.Close();
                table.Clear();
                isLoaded = true;
                IEnumerator enumerator = document.SelectSingleNode("N").SelectNodes("N").GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        XmlNode current = (XmlNode) enumerator.Current;
                        PowerTableEntry entry = new PowerTableEntry();
                        string str = current.Attributes["ID"].Value;
                        XmlNode node3 = current.SelectSingleNode("Name");
                        if (node3 != null)
                        {
                            entry.nameStrRef = StringTable.StringToInt(node3.InnerText);
                        }
                        XmlNode node4 = current.SelectSingleNode("Description");
                        if (node4 != null)
                        {
                            entry.descriptionStrRef = StringTable.StringToInt(node4.InnerText);
                        }
                        XmlNode node5 = current.SelectSingleNode("Icon");
                        if (node5 != null)
                        {
                            entry.Icon = node5.InnerText;
                        }
                        XmlNode node6 = current.SelectSingleNode("Family");
                        if (node6 != null)
                        {
                            entry.Family = node6.InnerText;
                        }
                        XmlNode node7 = current.SelectSingleNode("Rank");
                        if (node7 != null)
                        {
                            entry.Rank = int.Parse(node7.InnerText);
                        }
                        XmlNode node8 = current.SelectSingleNode("Index");
                        if (node8 != null)
                        {
                            entry.Index = int.Parse(node8.InnerText);
                        }
                        XmlNode node9 = current.SelectSingleNode("Requires");
                        if (node9 != null)
                        {
                            entry.Requires = node9.InnerText;
                        }
                        if (current.SelectSingleNode("Active") != null)
                        {
                            entry.Active = true;
                        }
                        if (current.SelectSingleNode("Modifier") != null)
                        {
                            entry.Modifier = true;
                        }
                        table[str] = entry;
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "PowerTable";
}

