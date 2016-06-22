using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class AdventurePathTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, AdventurePathTableEntry> table = new Dictionary<string, AdventurePathTableEntry>();

    public static AdventurePath Create(string ID)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Adventures/" + ID);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                AdventurePath component = obj3.GetComponent<AdventurePath>();
                if (component != null)
                {
                    AdventurePathTableEntry entry = table[ID];
                    if (entry != null)
                    {
                        component.DisplayName = entry.Name;
                        component.Set = entry.set;
                        component.RewardText = entry.Reward;
                    }
                }
                return component;
            }
        }
        return null;
    }

    public static AdventurePathTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<AdventurePathTableEntry>(i);
        }
        return null;
    }

    public static AdventurePathTableEntry Get(string ID)
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/AdventurePathTable", typeof(TextAsset));
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
                        AdventurePathTableEntry entry = new AdventurePathTableEntry();
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
                        XmlNode node6 = current.SelectSingleNode("Notes");
                        if (node6 != null)
                        {
                            entry.notesStrRef = StringTable.StringToInt(node6.InnerText);
                        }
                        XmlNode node7 = current.SelectSingleNode("Reward");
                        if (node7 != null)
                        {
                            entry.rewardStrRef = StringTable.StringToInt(node7.InnerText);
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
        "AdventurePathTable";
}

