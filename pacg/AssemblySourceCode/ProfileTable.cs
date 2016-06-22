using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class ProfileTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, ProfileTableEntry> table = new Dictionary<string, ProfileTableEntry>();

    public static ProfileTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<ProfileTableEntry>(i);
        }
        return null;
    }

    public static ProfileTableEntry Get(string ID)
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/ProfileTable", typeof(TextAsset));
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
                        ProfileTableEntry entry = new ProfileTableEntry();
                        string str = current.Attributes["ID"].Value;
                        XmlNode node3 = current.SelectSingleNode("Adventure");
                        if (node3 != null)
                        {
                            entry.AdventureID = node3.Attributes["ID"].Value;
                            entry.AdventurePathSet = node3.Attributes["Set"].Value;
                            entry.AdventureName = node3.InnerText;
                        }
                        XmlNode node4 = current.SelectSingleNode("Scenario");
                        if (node4 != null)
                        {
                            entry.ScenarioID = node4.Attributes["ID"].Value;
                            entry.ScenarioNumber = node4.Attributes["Number"].Value;
                            entry.ScenarioName = node4.InnerText;
                        }
                        int index = 0;
                        XmlNodeList list2 = current.SelectNodes("Character");
                        if ((list2 != null) && (list2.Count > 0))
                        {
                            entry.CharacterClasses = new string[list2.Count];
                            entry.CharacterIDs = new string[list2.Count];
                            entry.CharacterNames = new string[list2.Count];
                            IEnumerator enumerator2 = list2.GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    XmlNode node5 = (XmlNode) enumerator2.Current;
                                    entry.CharacterNames[index] = node5.InnerText;
                                    entry.CharacterIDs[index] = node5.Attributes["ID"].Value;
                                    entry.CharacterClasses[index] = node5.Attributes["Class"].Value;
                                    index++;
                                }
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
                        table[str] = entry;
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
        }
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "ProfileTable";
}

