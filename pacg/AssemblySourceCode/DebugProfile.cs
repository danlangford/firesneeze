using System;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

public class DebugProfile
{
    private static readonly char[] seperator = new char[] { ',' };
    private int slot = Constants.SAVE_SLOT_DEBUG;

    private Character AddCharacterToParty(string id)
    {
        Character member = CharacterTable.Create(id);
        if (member != null)
        {
            Party.Add(member);
        }
        return member;
    }

    public bool Load(string name)
    {
        TextAsset asset = (TextAsset) Resources.Load("Debug/Profiles/" + name, typeof(TextAsset));
        if (asset == null)
        {
            return false;
        }
        StringReader txtReader = new StringReader(asset.text);
        XmlDocument document = new XmlDocument();
        document.Load(txtReader);
        txtReader.Close();
        XmlNode node = document.SelectSingleNode("N");
        Campaign.Start("1B");
        Party.Clear();
        IEnumerator enumerator = node.SelectNodes("Character").GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                XmlNode current = (XmlNode) enumerator.Current;
                string id = current.Attributes["ID"].Value;
                Character character = this.AddCharacterToParty(id);
                if (character != null)
                {
                    XmlNode node3 = current.SelectSingleNode("Cards");
                    if (node3 != null)
                    {
                        string[] cardList = this.ParseCommaList(node3.InnerText);
                        character.Deck.SetCardList(cardList);
                    }
                    character.Deck.Shuffle();
                    XmlNode node4 = current.SelectSingleNode("Powers");
                    if (node4 != null)
                    {
                        foreach (string str2 in this.ParseCommaList(node4.InnerText))
                        {
                            character.Levelup(str2);
                        }
                    }
                    XmlNode node5 = current.SelectSingleNode("Proficiencies");
                    if (node5 != null)
                    {
                        string innerText = node5.InnerText;
                        if (innerText.Contains("LA"))
                        {
                            character.Levelup(ProficencyType.LightArmor);
                        }
                        if (innerText.Contains("HA"))
                        {
                            character.Levelup(ProficencyType.HeavyArmor);
                        }
                        if (innerText.Contains("WP"))
                        {
                            character.Levelup(ProficencyType.Weapons);
                        }
                    }
                    XmlNode node6 = current.SelectSingleNode("Hand");
                    if (node6 != null)
                    {
                        int delta = int.Parse(node6.InnerText) - character.HandSize;
                        character.Levelup(delta);
                    }
                    XmlNode node7 = current.SelectSingleNode("Skills");
                    if (node7 != null)
                    {
                        foreach (string str4 in this.ParseCommaList(node7.InnerText))
                        {
                            switch (str4)
                            {
                                case "STR":
                                    character.Levelup(AttributeType.Strength, 1);
                                    break;

                                case "DEX":
                                    character.Levelup(AttributeType.Dexterity, 1);
                                    break;

                                case "CON":
                                    character.Levelup(AttributeType.Constitution, 1);
                                    break;

                                case "INT":
                                    character.Levelup(AttributeType.Intelligence, 1);
                                    break;

                                case "WIS":
                                    character.Levelup(AttributeType.Wisdom, 1);
                                    break;

                                case "CHA":
                                    character.Levelup(AttributeType.Charisma, 1);
                                    break;
                            }
                        }
                    }
                    XmlNode node8 = current.SelectSingleNode("CardFeats");
                    if (node8 != null)
                    {
                        foreach (string str5 in this.ParseCommaList(node8.InnerText))
                        {
                            switch (str5)
                            {
                                case "WP":
                                    character.Levelup(CardType.Weapon, 1);
                                    break;

                                case "SP":
                                    character.Levelup(CardType.Spell, 1);
                                    break;

                                case "AR":
                                    character.Levelup(CardType.Armor, 1);
                                    break;

                                case "IT":
                                    character.Levelup(CardType.Item, 1);
                                    break;

                                case "AL":
                                    character.Levelup(CardType.Ally, 1);
                                    break;

                                case "BL":
                                    character.Levelup(CardType.Blessing, 1);
                                    break;
                            }
                        }
                    }
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
        IEnumerator enumerator2 = node.SelectNodes("Scenario").GetEnumerator();
        try
        {
            while (enumerator2.MoveNext())
            {
                XmlNode node9 = (XmlNode) enumerator2.Current;
                Campaign.SetScenarioComplete(node9.Attributes["ID"].Value);
            }
        }
        finally
        {
            IDisposable disposable2 = enumerator2 as IDisposable;
            if (disposable2 == null)
            {
            }
            disposable2.Dispose();
        }
        IEnumerator enumerator3 = node.SelectNodes("Adventure").GetEnumerator();
        try
        {
            while (enumerator3.MoveNext())
            {
                XmlNode node10 = (XmlNode) enumerator3.Current;
                Campaign.SetAdventureComplete(node10.Attributes["ID"].Value);
            }
        }
        finally
        {
            IDisposable disposable3 = enumerator3 as IDisposable;
            if (disposable3 == null)
            {
            }
            disposable3.Dispose();
        }
        return true;
    }

    private string[] ParseCommaList(string list) => 
        list.Replace(" ", string.Empty).Split(seperator, StringSplitOptions.RemoveEmptyEntries);

    public void Run()
    {
        Game.GameMode = GameModeType.Story;
        Game.Play(GameType.LocalSinglePlayer, this.slot, WindowType.Adventure, null, false);
    }

    public int Slot
    {
        get => 
            this.slot;
        set
        {
            this.slot = value;
        }
    }
}

