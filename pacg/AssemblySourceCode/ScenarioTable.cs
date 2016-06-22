using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class ScenarioTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, ScenarioTableEntry> table = new Dictionary<string, ScenarioTableEntry>();

    public static Scenario Create(string ID)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Scenarios/" + ID);
        if (prefab != null)
        {
            GameObject obj3 = Game.Instance.Create(prefab);
            if (obj3 != null)
            {
                Scenario component = obj3.GetComponent<Scenario>();
                if (component != null)
                {
                    ScenarioTableEntry entry = table[ID];
                    if (entry != null)
                    {
                        component.DisplayName = entry.Name;
                        component.Set = entry.set;
                        component.DisplayText = entry.Description;
                        component.DuringText = entry.Powers;
                        component.RewardText = entry.Reward;
                    }
                }
                return component;
            }
        }
        return null;
    }

    public static ScenarioTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<ScenarioTableEntry>(i);
        }
        return null;
    }

    public static ScenarioTableEntry Get(string ID)
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/ScenarioTable", typeof(TextAsset));
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
                        ScenarioTableEntry entry = new ScenarioTableEntry();
                        string str = current.Attributes["ID"].Value;
                        entry.id = str;
                        XmlNode node3 = current.SelectSingleNode("Set");
                        if (node3 != null)
                        {
                            entry.set = node3.InnerText;
                        }
                        XmlNode node4 = current.SelectSingleNode("Number");
                        if (node4 != null)
                        {
                            entry.number = int.Parse(node4.InnerText);
                        }
                        XmlNode node5 = current.SelectSingleNode("Name");
                        if (node5 != null)
                        {
                            entry.nameStrRef = StringTable.StringToInt(node5.InnerText);
                        }
                        XmlNode node6 = current.SelectSingleNode("Description");
                        if (node6 != null)
                        {
                            entry.descriptionStrRef = StringTable.StringToInt(node6.InnerText);
                        }
                        XmlNode node7 = current.SelectSingleNode("Powers");
                        if (node7 != null)
                        {
                            entry.powersStrRef = StringTable.StringToInt(node7.InnerText);
                        }
                        XmlNode node8 = current.SelectSingleNode("Reward");
                        if (node8 != null)
                        {
                            entry.rewardStrRef = StringTable.StringToInt(node8.InnerText);
                        }
                        XmlNode node9 = current.SelectSingleNode("Villain");
                        if (node9 != null)
                        {
                            entry.villainStrRef = StringTable.StringToInt(node9.InnerText);
                        }
                        table[str] = entry;
                        XmlNode node10 = current.SelectSingleNode("Henchmen");
                        if (node10 != null)
                        {
                            entry.henchmenStrRef = StringTable.StringToInt(node10.InnerText);
                        }
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
        "ScenarioTable";
}

