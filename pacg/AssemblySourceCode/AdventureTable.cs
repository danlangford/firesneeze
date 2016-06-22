using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class AdventureTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, AdventureTableEntry> table = new Dictionary<string, AdventureTableEntry>();

    public static Adventure Create(string ID)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Adventures/" + ID);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                Adventure component = obj3.GetComponent<Adventure>();
                if (component != null)
                {
                    AdventureTableEntry entry = table[ID];
                    if (entry != null)
                    {
                        component.DisplayName = entry.Name;
                        component.Set = entry.set;
                        component.DisplayText = entry.Description;
                        component.RewardText = entry.Reward;
                    }
                }
                return component;
            }
        }
        return null;
    }

    public static AdventureTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<AdventureTableEntry>(i);
        }
        return null;
    }

    public static AdventureTableEntry Get(string ID)
    {
        if (table.ContainsKey(ID))
        {
            return table[ID];
        }
        return null;
    }

    public static string Key(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Keys.ElementAt<string>(i);
        }
        return null;
    }

    public static void Load()
    {
        if (!isLoaded)
        {
            StringTableManager.Load(Name);
            TextAsset asset = (TextAsset) Resources.Load("Tables/AdventureTable", typeof(TextAsset));
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
                        AdventureTableEntry entry = new AdventureTableEntry();
                        string str = current.Attributes["ID"].Value;
                        entry.id = str;
                        XmlNode node3 = current.SelectSingleNode("Set");
                        if (node3 != null)
                        {
                            entry.set = node3.InnerText;
                        }
                        XmlNode node4 = current.SelectSingleNode("Name");
                        if (node4 != null)
                        {
                            entry.nameStrRef = StringTable.StringToInt(node4.InnerText);
                        }
                        XmlNode node5 = current.SelectSingleNode("Description");
                        if (node5 != null)
                        {
                            entry.descriptionStrRef = StringTable.StringToInt(node5.InnerText);
                        }
                        XmlNode node6 = current.SelectSingleNode("Reward");
                        if (node6 != null)
                        {
                            entry.rewardStrRef = StringTable.StringToInt(node6.InnerText);
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
        "AdventureTable";
}

