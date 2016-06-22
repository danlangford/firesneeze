using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class UbersTable
{
    private static bool isLoaded = false;
    private static List<UbersTableEntry> table = new List<UbersTableEntry>();

    public static UbersTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.ElementAt<UbersTableEntry>(i);
        }
        return null;
    }

    public static void Load()
    {
        if (!isLoaded)
        {
            TextAsset asset = (TextAsset) Resources.Load("Tables/UbersTable", typeof(TextAsset));
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
                        UbersTableEntry item = new UbersTableEntry();
                        string str = current.Attributes["ID"].Value;
                        string str2 = current.Attributes["TO"].Value;
                        item.from = str;
                        item.to = str2;
                        table.Add(item);
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
        "UbersTable";
}

