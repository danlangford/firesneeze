using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class ScenarioPowerTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, ScenarioPowerTableEntry> table = new Dictionary<string, ScenarioPowerTableEntry>();

    public static ScenarioPowerTableEntry Get(string ID)
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/ScenarioPowerTable", typeof(TextAsset));
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
                        ScenarioPowerTableEntry entry = new ScenarioPowerTableEntry();
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
        "ScenarioTable";
}

