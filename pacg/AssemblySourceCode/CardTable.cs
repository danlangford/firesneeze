using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public class CardTable
{
    private static readonly string[,] ArtFieldNames;
    private static bool isLoaded = false;
    private static readonly char[] seperator = new char[] { ',' };
    private static Dictionary<string, CardTableEntry> table = new Dictionary<string, CardTableEntry>();
    private static GameObject textTemplateLarge = null;
    private static GameObject textTemplateMedium = null;
    private static GameObject textTemplateSmall = null;

    static CardTable()
    {
        string[] textArray1 = new string[,] { { "Ally", "Ally_CheckHeader", "Ally_SkillText", "Ally_CheckValue", "Ally_OR" }, { "Armor", "Armor_CheckHeader", "Armor_SkillText", "Armor_CheckValue", "Armor_OR" }, { "Barrier", "Barrier_CheckHeader", "Barrier_SkillText", "Barrier_CheckValue", "Barrier_OR" }, { "Blessing", "Blessing_CheckHeader", "Blessing_SkillText", "Blessing_CheckValue", "Blessing_OR" }, { "Henchman", "Henchman_CheckHeader", "Henchman_SkillText", "Henchman_CheckValue", "Henchman_OR" }, { "Item", "Item_CheckHeader", "Item_SkillText", "Item_CheckValue", "Item_OR" }, { "Monster", "Monster_CheckHeader", "Monster_SkillText", "Monster_CheckValue", "Monster_OR" }, { "Spell", "Spell_CheckHeader", "Spell_SkillText", "Spell_CheckValue", "Spell_OR" }, { "Villain", "Villain_CheckHeader", "Villain_SkillText", "Villain_CheckValue", "Villain_OR" }, { "Weapon", "Weapon_CheckHeader", "Weapon_SkillText", "Weapon_CheckValue", "Weapon_OR" } };
        ArtFieldNames = textArray1;
    }

    private static void AdjustTextFieldAlignment(Transform template, string field, CardType subtype)
    {
        if (subtype != CardType.None)
        {
            Transform transform = template.FindChild(field);
            if (transform != null)
            {
                TextMesh component = transform.GetComponent<TextMesh>();
                if (component != null)
                {
                    component.anchor = TextAnchor.MiddleCenter;
                    component.alignment = TextAlignment.Center;
                }
            }
        }
    }

    private static void AdjustTextFieldPosition(Transform template, string field, CardType type, Vector3 offset)
    {
        if (offset != Vector3.zero)
        {
            Transform transform = template.FindChild(field);
            if (transform != null)
            {
                transform.position += offset;
            }
        }
    }

    public static void Create()
    {
        StringBuilder builder = new StringBuilder();
        TextAsset asset = (TextAsset) Resources.Load("Tables/CardTable", typeof(TextAsset));
        if (asset != null)
        {
            string iD = null;
            StringReader reader = new StringReader(asset.text);
            while (true)
            {
                string input = reader.ReadLine();
                if (input == null)
                {
                    break;
                }
                if (!input.Contains("<Traits>"))
                {
                    Match match = new Regex("\"([^\"]+)\"").Match(input);
                    if (match.Success)
                    {
                        iD = match.Groups[1].Value;
                    }
                    builder.AppendLine(input);
                    if (input.Contains("<Description>"))
                    {
                        string str3 = CreateTraitLine(iD);
                        if (str3 != null)
                        {
                            builder.AppendLine(str3);
                        }
                    }
                }
            }
        }
        File.WriteAllText(Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Resources"), "Tables"), "CardTable.txt"), builder.ToString());
    }

    public static Card Create(string ID) => 
        Create(ID, null, null);

    public static Card Create(string ID, string set, GameObject art = null)
    {
        if (!string.IsNullOrEmpty(ID))
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
                        component.GUID = Guid.NewGuid();
                        component.Show(false);
                        CardTableEntry entry = table[ID];
                        if (entry != null)
                        {
                            component.DisplayName = entry.Name;
                            component.DisplayText = entry.Description;
                            component.Set = string.IsNullOrEmpty(set) ? entry.set : set;
                            GameObject obj4 = (art == null) ? component.Art1 : art;
                            SetupCardFront(component, obj4, art == null);
                            SetupCardBack(component, component.Art2);
                            SetupCardType(component);
                        }
                    }
                    Geometry.SetLayerRecursively(go, Constants.LAYER_CARD);
                    return component;
                }
            }
        }
        return null;
    }

    private static string CreateTraitLine(string ID)
    {
        if ((ID != null) && (ID.Length >= 6))
        {
            GameObject obj2 = FindCardBlueprint(ID);
            if (obj2 != null)
            {
                Card component = obj2.GetComponent<Card>();
                if (component != null)
                {
                    string str = null;
                    for (int i = 0; i < component.Traits.Length; i++)
                    {
                        string str2 = ((int) component.Traits[i]).ToString();
                        if (str == null)
                        {
                            str = str2;
                        }
                        else
                        {
                            str = str + "," + str2;
                        }
                    }
                    return ("  <Traits>" + str + "</Traits>");
                }
            }
            Debug.LogWarning("Traits Missing: " + ID);
        }
        return null;
    }

    private static GameObject FindCardBlueprint(string ID)
    {
        string str = "Blueprints/Cards/";
        GameObject obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        string oldValue = ID[2].ToString() + ID[3].ToString() + ID[4].ToString();
        ID = ID.Replace(oldValue, "1B_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("1B_", "1C_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("1C_", "11_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("11_", "12_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("12_", "13_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("13_", "14_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        ID = ID.Replace("14_", "15_");
        obj2 = Resources.Load<GameObject>(str + ID);
        if (obj2 != null)
        {
            return obj2;
        }
        return null;
    }

    public static CardTableEntry Get(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Values.ElementAt<CardTableEntry>(i);
        }
        return null;
    }

    public static CardTableEntry Get(string ID)
    {
        if (table.ContainsKey(ID))
        {
            return table[ID];
        }
        return null;
    }

    private static int GetArtFieldIndex(CardType type)
    {
        if (type != CardType.Ally)
        {
            if (type == CardType.Armor)
            {
                return 1;
            }
            if (type == CardType.Barrier)
            {
                return 2;
            }
            if (type == CardType.Blessing)
            {
                return 3;
            }
            if (type == CardType.Henchman)
            {
                return 4;
            }
            if (type == CardType.Item)
            {
                return 5;
            }
            if (type == CardType.Monster)
            {
                return 6;
            }
            if (type == CardType.Spell)
            {
                return 7;
            }
            if (type == CardType.Villain)
            {
                return 8;
            }
            if (type == CardType.Weapon)
            {
                return 9;
            }
            if (type == CardType.Loot)
            {
                return 5;
            }
        }
        return 0;
    }

    private static bool GetCardCheck1OccupiesTwoBoxes(Card card, CardPropertyAlternateCheck alternateCheck)
    {
        if (alternateCheck != null)
        {
            return (alternateCheck.GetNumberOfLinesInCheck() > 1);
        }
        int cardLowestCheck = GetCardLowestCheck(card, alternateCheck);
        bool flag = false;
        for (int i = 0; i < card.Checks1.Length; i++)
        {
            if (card.Checks1[i].Rank != cardLowestCheck)
            {
                flag = true;
            }
        }
        return flag;
    }

    private static string GetCardCheckBubbleName(int number)
    {
        if (number == 1)
        {
            return "numbers_check_1";
        }
        if (number == 2)
        {
            return "numbers_check_2";
        }
        if (number == 3)
        {
            return "numbers_check_3";
        }
        if (number == 4)
        {
            return "numbers_check_4";
        }
        if (number == 5)
        {
            return "numbers_check_5";
        }
        if (number == 6)
        {
            return "numbers_check_6";
        }
        if (number == 7)
        {
            return "numbers_check_7";
        }
        if (number == 8)
        {
            return "numbers_check_8";
        }
        if (number == 9)
        {
            return "numbers_check_9";
        }
        if (number == 10)
        {
            return "numbers_check_10";
        }
        if (number == 11)
        {
            return "numbers_check_11";
        }
        if (number == 12)
        {
            return "numbers_check_12";
        }
        if (number == 13)
        {
            return "numbers_check_13";
        }
        if (number == 14)
        {
            return "numbers_check_14";
        }
        if (number == 15)
        {
            return "numbers_check_15";
        }
        if (number == 0x10)
        {
            return "numbers_check_16";
        }
        if (number == 0x11)
        {
            return "numbers_check_17";
        }
        if (number == 0x12)
        {
            return "numbers_check_18";
        }
        if (number == 0x13)
        {
            return "numbers_check_19";
        }
        if (number == 20)
        {
            return "numbers_check_20";
        }
        if (number == 20)
        {
            return "numbers_check_20";
        }
        if (number == 0x15)
        {
            return "numbers_check_21";
        }
        if (number == 0x16)
        {
            return "numbers_check_22";
        }
        if (number == 0x17)
        {
            return "numbers_check_23";
        }
        if (number == 0x18)
        {
            return "numbers_check_24";
        }
        if (number == 0x19)
        {
            return "numbers_check_25";
        }
        if (number == 0x1a)
        {
            return "numbers_check_26";
        }
        if (number == 0x1b)
        {
            return "numbers_check_27";
        }
        if (number == 0x1c)
        {
            return "numbers_check_28";
        }
        if (number == 0x1d)
        {
            return "numbers_check_29";
        }
        if (number == 30)
        {
            return "numbers_check_30";
        }
        return null;
    }

    private static int GetCardCheckRank(Card card, int line, CardPropertyAlternateCheck alternateCheck)
    {
        int cardLowestCheck = GetCardLowestCheck(card, alternateCheck);
        int rank = 0;
        bool flag = false;
        for (int i = 0; i < card.Checks1.Length; i++)
        {
            if ((line == 1) && (card.Checks1[i].Rank == cardLowestCheck))
            {
                rank = card.Checks1[i].Rank;
                break;
            }
            if (card.Checks1[i].Rank != cardLowestCheck)
            {
                flag = true;
                if (line == 2)
                {
                    rank = card.Checks1[i].Rank;
                    break;
                }
            }
        }
        if ((((line == 2) && !flag) || ((line == 3) && flag)) && ((card.Checks2 != null) && (card.Checks2.Length > 0)))
        {
            rank = card.Checks2[0].Rank;
        }
        return rank;
    }

    private static int GetCardCheckSkillCount(Card card, int line, CardPropertyAlternateCheck alternateCheck)
    {
        int num = 0;
        int cardLowestCheck = GetCardLowestCheck(card, alternateCheck);
        bool flag = false;
        if ((cardLowestCheck == -1) && (line == 1))
        {
            return alternateCheck.GetNumberOfLinesInCheck();
        }
        for (int i = 0; i < card.Checks1.Length; i++)
        {
            if ((line == 1) && (card.Checks1[i].Rank == cardLowestCheck))
            {
                num++;
            }
            if (card.Checks1[i].Rank != cardLowestCheck)
            {
                flag = true;
                if (line == 2)
                {
                    num++;
                }
            }
        }
        if (((line == 2) && !flag) || ((line == 3) && flag))
        {
            if (card.Checks2 == null)
            {
                return num;
            }
            for (int j = 0; j < card.Checks2.Length; j++)
            {
                num++;
            }
        }
        return num;
    }

    private static string GetCardCheckSkillText(Card card, int line, CardPropertyAlternateCheck alternateCheck)
    {
        StringBuilder builder = new StringBuilder();
        int cardLowestCheck = GetCardLowestCheck(card, alternateCheck);
        bool flag = false;
        if (((cardLowestCheck == -1) && (line == 1)) && (alternateCheck != null))
        {
            builder.AppendLine(alternateCheck.CheckText.ToString());
        }
        else
        {
            for (int i = 0; i < card.Checks1.Length; i++)
            {
                if ((line == 1) && (card.Checks1[i].Rank == cardLowestCheck))
                {
                    builder.AppendLine(card.Checks1[i].skill.ToText().ToUpper());
                }
                if (card.Checks1[i].Rank != cardLowestCheck)
                {
                    flag = true;
                    if (line == 2)
                    {
                        builder.AppendLine(card.Checks1[i].skill.ToText().ToUpper());
                    }
                }
            }
        }
        if ((((line == 2) && !flag) || ((line == 3) && flag)) && (card.Checks2 != null))
        {
            for (int j = 0; j < card.Checks2.Length; j++)
            {
                builder.AppendLine(card.Checks2[j].skill.ToText().ToUpper());
            }
        }
        return builder.ToString();
    }

    private static string GetCardDeckBubbleName(string set)
    {
        if (set == "B")
        {
            return "numbers_letters_deck_11";
        }
        if (set == "C")
        {
            return "numbers_letters_deck_12";
        }
        if (set == "1")
        {
            return "numbers_letters_deck_1";
        }
        if (set == "2")
        {
            return "numbers_letters_deck_2";
        }
        if (set == "3")
        {
            return "numbers_letters_deck_3";
        }
        if (set == "4")
        {
            return "numbers_letters_deck_4";
        }
        if (set == "5")
        {
            return "numbers_letters_deck_5";
        }
        if (set == "6")
        {
            return "numbers_letters_deck_6";
        }
        return null;
    }

    private static int GetCardLowestCheck(Card card, CardPropertyAlternateCheck altCheck)
    {
        if (altCheck != null)
        {
            return -1;
        }
        int b = (card.Checks1.Length <= 0) ? 0 : card.Checks1[0].Rank;
        for (int i = 0; i < card.Checks1.Length; i++)
        {
            b = Mathf.Min(card.Checks1[i].Rank, b);
        }
        return b;
    }

    private static string GetCardTraitText(Card card)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < card.Traits.Length; i++)
        {
            builder.AppendLine(card.Traits[i].ToText().ToUpper());
        }
        return builder.ToString();
    }

    private static int GetCardTypeCount(Card card)
    {
        if (((card.Type != CardType.Villain) && (card.Type != CardType.Henchman)) && (card.Type != CardType.Loot))
        {
            return 0;
        }
        return 1;
    }

    private static float GetCheckHeaderFieldOffset(CardType type)
    {
        if (type == CardType.Henchman)
        {
            return -0.5f;
        }
        if (type == CardType.Villain)
        {
            return -0.5f;
        }
        return 0f;
    }

    public static string GetKey(int i)
    {
        if ((i >= 0) && (i < Count))
        {
            return table.Keys.ElementAt<string>(i);
        }
        return null;
    }

    private static GameObject GetTextTemplate(GameObject art, Card card)
    {
        if (card.ArtFormat == CardFormatType.Short)
        {
            return Geometry.CreateChildObject(art, textTemplateSmall, "Text");
        }
        if (card.ArtFormat == CardFormatType.Normal)
        {
            return Geometry.CreateChildObject(art, textTemplateMedium, "Text");
        }
        return Geometry.CreateChildObject(art, textTemplateLarge, "Text");
    }

    public static void Load()
    {
        if (textTemplateSmall == null)
        {
            textTemplateSmall = Resources.Load<GameObject>("Art/Templates/CardTextSmall");
        }
        if (textTemplateMedium == null)
        {
            textTemplateMedium = Resources.Load<GameObject>("Art/Templates/CardText");
        }
        if (textTemplateLarge == null)
        {
            textTemplateLarge = Resources.Load<GameObject>("Art/Templates/CardTextLarge");
        }
        if (!isLoaded)
        {
            StringTableManager.Load(Name);
            TextAsset asset = (TextAsset) Resources.Load("Tables/CardTable", typeof(TextAsset));
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
                        CardTableEntry entry = new CardTableEntry();
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
                        XmlNode node6 = current.SelectSingleNode("Traits");
                        if (node6 != null)
                        {
                            entry.traits = ParseTraitList(node6.InnerText);
                        }
                        XmlNode node7 = current.SelectSingleNode("Type2");
                        if (node7 != null)
                        {
                            entry.type2 = LookupCardType(node7.InnerText);
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

    public static bool LookupCardBooster(string id) => 
        ((!string.IsNullOrEmpty(id) && (id.Length > 4)) && (id[2] == '0'));

    public static bool LookupCardPromo(string id) => 
        ((!string.IsNullOrEmpty(id) && (id.Length > 4)) && (id[3] == 'P'));

    public static string LookupCardSet(string ID)
    {
        if (!string.IsNullOrEmpty(ID))
        {
            CardTableEntry entry = Get(ID);
            if (entry != null)
            {
                return entry.set;
            }
        }
        return null;
    }

    public static CardType LookupCardSubType(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            CardTableEntry entry = Get(id);
            if (entry != null)
            {
                return entry.type2;
            }
        }
        return CardType.None;
    }

    public static TraitType[] LookupCardTraits(string ID)
    {
        if (!string.IsNullOrEmpty(ID))
        {
            CardTableEntry entry = Get(ID);
            if (entry != null)
            {
                return entry.traits;
            }
        }
        return null;
    }

    public static CardType LookupCardType(string id)
    {
        if (!string.IsNullOrEmpty(id) && (id.Length >= 2))
        {
            if ((id[0] == 'A') && (id[1] == 'L'))
            {
                return CardType.Ally;
            }
            if ((id[0] == 'A') && (id[1] == 'R'))
            {
                return CardType.Armor;
            }
            if ((id[0] == 'B') && (id[1] == 'X'))
            {
                return CardType.Barrier;
            }
            if ((id[0] == 'B') && (id[1] == 'L'))
            {
                return CardType.Blessing;
            }
            if ((id[0] == 'H') && (id[1] == 'E'))
            {
                return CardType.Henchman;
            }
            if ((id[0] == 'I') && (id[1] == 'T'))
            {
                return CardType.Item;
            }
            if ((id[0] == 'M') && (id[1] == 'O'))
            {
                return CardType.Monster;
            }
            if ((id[0] == 'S') && (id[1] == 'P'))
            {
                return CardType.Spell;
            }
            if ((id[0] == 'V') && (id[1] == 'L'))
            {
                return CardType.Villain;
            }
            if ((id[0] == 'W') && (id[1] == 'P'))
            {
                return CardType.Weapon;
            }
            if ((id[0] == 'L') && (id[1] == 'T'))
            {
                return CardType.Loot;
            }
            if ((id[0] == 'C') && (id[1] == 'H'))
            {
                return CardType.Character;
            }
            if ((id[0] == 'S') && (id[1] == 'C'))
            {
                return CardType.Scenario;
            }
        }
        return CardType.None;
    }

    private static float PaintCardTypeHeader(Transform art, Transform text, CardType type, float push)
    {
        if (text != null)
        {
            text.localPosition = new Vector3(2.85f, 3.65f + push, -0.01f);
            push -= 0.5f;
            GuiLabel component = text.GetComponent<GuiLabel>();
            if (component != null)
            {
                object[] objArray1 = new object[] { UI.Text(0x106), '\n', type.ToText().ToUpper(), '\n' };
                component.Text = string.Concat(objArray1);
            }
        }
        return push;
    }

    private static float PaintSkillCheckHeader(Transform art, float push)
    {
        if (art != null)
        {
            art.gameObject.SetActive(true);
            art.localPosition = new Vector3(2.9f, 3f + push, -0.1f);
        }
        return push;
    }

    private static float PaintSkillCheckOr(Transform art, float push)
    {
        if (art != null)
        {
            art.gameObject.SetActive(true);
            art.localPosition = new Vector3(2.88f, 2.89f + push, -0.1f);
        }
        return push;
    }

    private static float PaintSkillCheckRank(Transform art, Transform text, int rank, float push)
    {
        if ((art != null) && (text != null))
        {
            art.gameObject.SetActive(true);
            art.localPosition = new Vector3(2.88f, 2f + push, 0.01f);
            text.localPosition = art.localPosition;
            Transform transform = text.FindChild(GetCardCheckBubbleName(rank));
            if (transform != null)
            {
                transform.gameObject.SetActive(true);
                transform.position = new Vector3(art.localPosition.x - 0.056f, art.localPosition.y + 0.78f, 0f);
            }
        }
        return push;
    }

    private static float PaintSkillCheckText(Transform art, Transform text, string skills, int lines, float push)
    {
        if ((art != null) && (text != null))
        {
            if (lines <= 1)
            {
                art.gameObject.SetActive(true);
                art.localScale = new Vector3(1f, 1.2f, 1f);
                art.localPosition = new Vector3(2.88f, 2.63f + push, 0.02f);
                push = push;
            }
            else if (lines == 2)
            {
                art.gameObject.SetActive(true);
                art.localScale = new Vector3(1f, 1.7f, 1f);
                art.localPosition = new Vector3(2.88f, 2.56f + push, 0.02f);
                push -= 0.18f;
            }
            else if (lines == 3)
            {
                art.gameObject.SetActive(true);
                art.localScale = new Vector3(1f, 2.2f, 1f);
                art.localPosition = new Vector3(2.88f, 2.48f + push, 0.02f);
                push -= 0.32f;
            }
            else if (lines >= 4)
            {
                art.gameObject.SetActive(true);
                art.localScale = new Vector3(1f, 3f, 1f);
                art.localPosition = new Vector3(2.88f, 2.38f + push, 0.02f);
                push -= 0.54f;
            }
            text.localPosition = new Vector3(art.localPosition.x - 0.08f, art.localPosition.y + 0.63f, 0f);
            GuiLabel component = text.GetComponent<GuiLabel>();
            if (component != null)
            {
                component.Text = skills;
            }
        }
        return push;
    }

    private static TraitType[] ParseTraitList(string traitString)
    {
        string[] strArray = traitString.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
        if (strArray == null)
        {
            return null;
        }
        TraitType[] typeArray = new TraitType[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            typeArray[i] = (TraitType) StringTable.StringToInt(strArray[i]);
        }
        return typeArray;
    }

    private static void SetCardDeckField(Transform template, string field, string set, float offset)
    {
        Transform transform = template.transform.FindChild(field);
        if (transform != null)
        {
            Transform transform2 = transform.FindChild(GetCardDeckBubbleName(set));
            if (transform2 != null)
            {
                Transform transform1 = transform2.transform;
                transform1.position += new Vector3(0f, offset, 0f);
                transform2.gameObject.SetActive(true);
            }
        }
    }

    private static void SetCardTextField(Transform template, string field, string text, float offset)
    {
        Transform transform = template.FindChild(field);
        if (transform != null)
        {
            GuiLabel component = transform.GetComponent<GuiLabel>();
            if (component != null)
            {
                component.Text = text;
                Transform transform1 = component.transform;
                transform1.position += new Vector3(0f, offset, 0f);
            }
        }
    }

    private static void SetCheckFields(Transform template, Card card)
    {
        CardPropertyAlternateCheck component = card.GetComponent<CardPropertyAlternateCheck>();
        int cardTypeCount = GetCardTypeCount(card);
        bool flag = GetCardCheck1OccupiesTwoBoxes(card, component);
        int lines = GetCardCheckSkillCount(card, 1, component);
        int num3 = GetCardCheckSkillCount(card, 2, component);
        int num4 = 0;
        if (num3 > 0)
        {
            num4 = GetCardCheckSkillCount(card, 3, component);
        }
        int artFieldIndex = GetArtFieldIndex(card.Type);
        float push = 0f;
        if (cardTypeCount > 0)
        {
            push = PaintCardTypeHeader(null, template.FindChild("subtype"), card.SubType, push);
        }
        if (lines > 0)
        {
            Transform transform = template.transform.FindChild("box_1");
            if (transform != null)
            {
                Transform transform2 = transform.FindChild(ArtFieldNames[artFieldIndex, 0]);
                if (transform2 != null)
                {
                    push = PaintSkillCheckHeader(transform2.FindChild(ArtFieldNames[artFieldIndex, 1]), push);
                    push = PaintSkillCheckText(transform2.FindChild(ArtFieldNames[artFieldIndex, 2]), template.FindChild("check_type_1"), GetCardCheckSkillText(card, 1, component), lines, push);
                    if (component == null)
                    {
                        push = PaintSkillCheckRank(transform2.FindChild(ArtFieldNames[artFieldIndex, 3]), template.FindChild("check_circle_1"), GetCardCheckRank(card, 1, component), push);
                    }
                    else
                    {
                        push += 0.9f;
                    }
                }
            }
        }
        if (num3 > 0)
        {
            if (flag)
            {
                SetCardTextField(template.transform, "Label - OR THEN", UI.Text(260), push);
            }
            else
            {
                SetCardTextField(template.transform, "Label - OR THEN", UI.Text(0x105), push);
            }
            Transform transform3 = template.transform.FindChild("box_2");
            if (transform3 != null)
            {
                Transform transform4 = transform3.FindChild(ArtFieldNames[artFieldIndex, 0]);
                if (transform4 != null)
                {
                    if (component == null)
                    {
                        push -= 1.5f;
                    }
                    else
                    {
                        push -= 1.4431f;
                    }
                    push = PaintSkillCheckOr(transform4.FindChild(ArtFieldNames[artFieldIndex, 4]), push);
                    push = PaintSkillCheckText(transform4.FindChild(ArtFieldNames[artFieldIndex, 2]), template.FindChild("check_type_2"), GetCardCheckSkillText(card, 2, component), num3, push);
                    push = PaintSkillCheckRank(transform4.FindChild(ArtFieldNames[artFieldIndex, 3]), template.FindChild("check_circle_2"), GetCardCheckRank(card, 2, component), push);
                }
            }
        }
        if (num4 > 0)
        {
            if (!flag)
            {
                SetCardTextField(template.transform, "Label2 - OR THEN", UI.Text(260), push);
            }
            else
            {
                SetCardTextField(template.transform, "Label2 - OR THEN", UI.Text(0x105), push);
            }
            Transform transform5 = template.transform.FindChild("box_3");
            if (transform5 != null)
            {
                Transform transform6 = transform5.FindChild(ArtFieldNames[artFieldIndex, 0]);
                if (transform6 != null)
                {
                    push -= 1.5f;
                    push = PaintSkillCheckOr(transform6.FindChild(ArtFieldNames[artFieldIndex, 4]), push);
                    push = PaintSkillCheckText(transform6.FindChild(ArtFieldNames[artFieldIndex, 2]), template.FindChild("check_type_3"), GetCardCheckSkillText(card, 3, component), num4, push);
                    push = PaintSkillCheckRank(transform6.FindChild(ArtFieldNames[artFieldIndex, 3]), template.FindChild("check_circle_3"), GetCardCheckRank(card, 3, component), push);
                }
            }
        }
        if (((lines == 0) && (num3 == 0)) && (card.Type != CardType.Loot))
        {
            Transform transform7 = template.transform.FindChild("box_1");
            if (transform7 != null)
            {
                Transform transform8 = transform7.FindChild(ArtFieldNames[artFieldIndex, 0]);
                if (transform8 != null)
                {
                    Transform text = template.FindChild("check_type_1");
                    push = PaintSkillCheckHeader(transform8.FindChild(ArtFieldNames[artFieldIndex, 1]), push);
                    push = PaintSkillCheckText(transform8.FindChild(ArtFieldNames[artFieldIndex, 2]), text, "NONE", 1, push);
                    text.localPosition = new Vector3(text.localPosition.x, text.localPosition.y + 0.1f, 0f);
                }
            }
        }
    }

    private static void SetHeaderFields(Transform template, Card card)
    {
        SetCardTextField(template.transform, "Label - POWERS", UI.Text(0x101), 0f);
        float checkHeaderFieldOffset = GetCheckHeaderFieldOffset(card.Type);
        if (card.IsBoon())
        {
            SetCardTextField(template.transform, "Label - Check To", UI.Text(0x102), checkHeaderFieldOffset);
        }
        if (card.IsBane())
        {
            SetCardTextField(template.transform, "Label - Check To", UI.Text(0x103), checkHeaderFieldOffset);
        }
    }

    private static void SetupCardBack(Card card, GameObject art2)
    {
        GameObject parent = Geometry.CreateChild(card.gameObject, "Back");
        if (art2 != null)
        {
            Geometry.CreateChildObject(parent, art2, "Art");
        }
    }

    private static void SetupCardFrameRarity(Card card, GameObject parent)
    {
        if (card.Rarity != RarityType.Common)
        {
            string path = string.Empty;
            if (card.Rarity == RarityType.Uncommon)
            {
                path = "Art/Cards/Rarity_01";
            }
            else if (card.Rarity == RarityType.Rare)
            {
                path = "Art/Cards/Rarity_02";
            }
            else if (card.Rarity == RarityType.Epic)
            {
                path = "Art/Cards/Rarity_03";
            }
            else if (card.Rarity == RarityType.Legendary)
            {
                path = "Art/Cards/Rarity_04";
            }
            if (path.Length > 0)
            {
                UnityEngine.Object original = Resources.Load(path);
                if (original != null)
                {
                    GameObject obj3 = UnityEngine.Object.Instantiate(original) as GameObject;
                    if (obj3 != null)
                    {
                        obj3.transform.parent = parent.transform;
                    }
                }
            }
        }
    }

    private static void SetupCardFront(Card card, GameObject art1, bool fullText)
    {
        GameObject parent = Geometry.CreateChild(card.gameObject, "Front");
        if (art1 != null)
        {
            GameObject obj3 = Geometry.CreateChildObject(parent, art1, "Art");
            SetupCardFrameRarity(card, obj3);
            GameObject textTemplate = GetTextTemplate(obj3, card);
            if (textTemplate != null)
            {
                SetCardTextField(textTemplate.transform, "title", card.DisplayName.ToUpper(), 0f);
                if (fullText)
                {
                    SetCardDeckField(textTemplate.transform, "set", card.Set, 0f);
                    SetCardTextField(textTemplate.transform, "type", card.Type.ToText().ToUpper(), 0f);
                    SetCardTextField(textTemplate.transform, "powers", card.DisplayText, 0f);
                    SetCardTextField(textTemplate.transform, "traits", GetCardTraitText(card), 0f);
                    SetHeaderFields(textTemplate.transform, card);
                    SetCheckFields(textTemplate.transform, card);
                    AdjustTextFieldAlignment(textTemplate.transform, "subtype", card.SubType);
                }
            }
        }
    }

    private static void SetupCardType(Card card)
    {
        if (card.Type == CardType.Loot)
        {
            card.Type = card.SubType;
        }
    }

    public static int Count =>
        table.Count;

    public static string Name =>
        "CardTable";
}

