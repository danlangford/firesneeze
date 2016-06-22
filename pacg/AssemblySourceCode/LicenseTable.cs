using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class LicenseTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, LicenseTableEntry> table = new Dictionary<string, LicenseTableEntry>();

    public static LicenseTableEntry Get(string ID)
    {
        if ((ID != null) && table.ContainsKey(ID))
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
            TextAsset asset = (TextAsset) Resources.Load("Tables/LicenseTable", typeof(TextAsset));
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
                        LicenseTableEntry entry = new LicenseTableEntry();
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
                        XmlNode node6 = current.SelectSingleNode("Art");
                        if (node6 != null)
                        {
                            entry.Art = node6.InnerText;
                        }
                        XmlNode node7 = current.SelectSingleNode("Type");
                        if (node7 != null)
                        {
                            entry.Type = ParseLicenseType(node7.InnerText);
                        }
                        XmlNode node8 = current.SelectSingleNode("Status");
                        if (node8 != null)
                        {
                            entry.Available = ParseLicenseStatus(node8.InnerText);
                        }
                        XmlNode node9 = current.SelectSingleNode("Date");
                        if (node9 != null)
                        {
                            entry.Date = node9.InnerText;
                        }
                        XmlNode node10 = current.SelectSingleNode("Nickname");
                        if (node10 != null)
                        {
                            entry.Nickname = node10.InnerText;
                        }
                        XmlNodeList list2 = current.SelectNodes("Preview");
                        entry.Preview = new string[list2.Count];
                        for (int i = 0; i < list2.Count; i++)
                        {
                            entry.Preview[i] = list2[i].InnerText;
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

    private static bool ParseLicenseStatus(string status) => 
        (status == "A");

    private static LicenseType ParseLicenseType(string id)
    {
        if (id == "A")
        {
            return LicenseType.Adventure;
        }
        if (id == "C")
        {
            return LicenseType.Character;
        }
        if (id == "IG")
        {
            return LicenseType.Gold;
        }
        if (id == "SG")
        {
            return LicenseType.Gold;
        }
        if (id == "SP")
        {
            return LicenseType.Special;
        }
        if (id == "TC")
        {
            return LicenseType.TreasurePurchase;
        }
        return LicenseType.None;
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "LicenseTable";
}

