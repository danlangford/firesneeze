using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class RoleTable
{
    private static bool isLoaded = false;
    private static readonly char[] seperator = new char[] { ',' };
    private static Dictionary<string, RoleTableEntry> table = new Dictionary<string, RoleTableEntry>();

    private static ProficencyType ConvertStringToProficiency(string prof)
    {
        switch (prof.Replace(" ", string.Empty))
        {
            case "WP":
                return ProficencyType.Weapons;

            case "LA":
                return ProficencyType.LightArmor;

            case "HA":
                return ProficencyType.HeavyArmor;
        }
        return ProficencyType.None;
    }

    public static RoleTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<RoleTableEntry>(i);
        }
        return null;
    }

    public static RoleTableEntry Get(string ID)
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/RoleTable", typeof(TextAsset));
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
                        RoleTableEntry entry = new RoleTableEntry();
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
                        XmlNode node5 = current.SelectSingleNode("Hand");
                        if (node5 != null)
                        {
                            entry.HandSize = int.Parse(node5.InnerText);
                        }
                        XmlNode node6 = current.SelectSingleNode("Profs");
                        if (node6 != null)
                        {
                            entry.Proficiencies = ParseProfs(node6.InnerText);
                        }
                        XmlNode node7 = current.SelectSingleNode("Powers");
                        if (node7 != null)
                        {
                            entry.Powers = ParsePowers(node7.InnerText);
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

    private static string[] ParsePowers(string powers) => 
        powers.Replace(" ", string.Empty).Split(seperator, StringSplitOptions.RemoveEmptyEntries);

    private static ProficencyType[] ParseProfs(string profs)
    {
        string[] strArray = profs.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray == null)
        {
            return null;
        }
        ProficencyType[] typeArray = new ProficencyType[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            typeArray[i] = ConvertStringToProficiency(strArray[i]);
        }
        return typeArray;
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "RoleTable";
}

