using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class LocationTable
{
    private static bool isLoaded = false;
    private static Dictionary<string, LocationTableEntry> table = new Dictionary<string, LocationTableEntry>();
    private static GameObject textTemplate1 = null;
    private static GameObject textTemplate2 = null;

    public static Card CreateCard(string ID)
    {
        GameObject prefab = Resources.Load<GameObject>("Blueprints/Cards/" + ID);
        if (prefab != null)
        {
            GameObject go = Game.Instance.Create(prefab);
            if (go != null)
            {
                Card component = go.GetComponent<Card>();
                if (component != null)
                {
                    component.Show(false);
                    LocationTableEntry row = table[ID];
                    if (row != null)
                    {
                        component.DisplayName = row.Name;
                        component.Set = row.set;
                        SetupCardFront(component, row);
                        SetupCardBack(component, row);
                    }
                }
                Geometry.SetLayerRecursively(go, Constants.LAYER_CARD);
                return component;
            }
        }
        return null;
    }

    public static LocationTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<LocationTableEntry>(i);
        }
        return null;
    }

    public static LocationTableEntry Get(string ID)
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
        if (textTemplate1 == null)
        {
            textTemplate1 = Resources.Load<GameObject>("Art/Templates/LocationText1");
        }
        if (textTemplate2 == null)
        {
            textTemplate2 = Resources.Load<GameObject>("Art/Templates/LocationText2");
        }
        if (!isLoaded)
        {
            StringTableManager.Load(Name);
            TextAsset asset = (TextAsset) Resources.Load("Tables/LocationTable", typeof(TextAsset));
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
                        LocationTableEntry entry = new LocationTableEntry();
                        string str = current.Attributes["ID"].Value;
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
                        XmlNode node5 = current.SelectSingleNode("Location");
                        if (node5 != null)
                        {
                            entry.locationStrRef = StringTable.StringToInt(node5.InnerText);
                        }
                        XmlNode node6 = current.SelectSingleNode("Closing");
                        if (node6 != null)
                        {
                            entry.closingStrRef = StringTable.StringToInt(node6.InnerText);
                        }
                        XmlNode node7 = current.SelectSingleNode("Closed");
                        if (node7 != null)
                        {
                            entry.closedStrRef = StringTable.StringToInt(node7.InnerText);
                        }
                        XmlNode node8 = current.SelectSingleNode("Deck");
                        if (node8 != null)
                        {
                            entry.deck = node8.InnerText;
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

    private static void SetCardTextField(GameObject artTemplate, string artFieldName, GameObject textTemplate, string textFieldName, string text)
    {
        Transform transform = textTemplate.transform.FindChild(textFieldName);
        if (transform != null)
        {
            GuiLabel component = transform.GetComponent<GuiLabel>();
            if (component != null)
            {
                component.Text = text;
            }
            Transform transform2 = artTemplate.transform.FindChild(artFieldName);
            if (transform2 != null)
            {
                transform.position = new Vector3(transform2.position.x, transform2.position.y, transform.position.z);
            }
        }
    }

    private static void SetupCardBack(Card card, LocationTableEntry row)
    {
        GameObject parent = Geometry.CreateChild(card.gameObject, "Back");
        if (card.Art2 != null)
        {
            Geometry.CreateChildObject(parent, card.Art2, "Art");
        }
        if ((textTemplate2 != null) && (Geometry.CreateChildObject(parent, textTemplate2, "Text") != null))
        {
        }
    }

    private static void SetupCardFront(Card card, LocationTableEntry row)
    {
        GameObject parent = Geometry.CreateChild(card.gameObject, "Front");
        if (card.Art1 != null)
        {
            GameObject obj3 = Geometry.CreateChildObject(parent, card.Art1, "Art");
            if (textTemplate1 != null)
            {
                GameObject textTemplate = Geometry.CreateChildObject(obj3, textTemplate1, "Text");
                if (textTemplate != null)
                {
                    SetCardTextField(obj3, "Text_AtThisLocation", textTemplate, "location", row.Location);
                    SetCardTextField(obj3, "Text_WhenClosing", textTemplate, "closing", row.Closing);
                    SetCardTextField(obj3, "Text_WhenPermanentlyClosed", textTemplate, "closed", row.Closed);
                }
            }
        }
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "LocationTable";
}

