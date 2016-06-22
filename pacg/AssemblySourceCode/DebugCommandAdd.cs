using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DebugCommandAdd : DebugCommand
{
    [CompilerGenerated]
    private static Func<char, bool> <>f__am$cache1;
    [CompilerGenerated]
    private static Func<char, bool> <>f__am$cache3;
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map0;
    private readonly char[] regexAny = new char[] { '*' };

    private string CleanName(string cardName)
    {
        if (cardName.IndexOfAny("`~!@#$%^&*()_+-=[]{};':\"\\|,.<>/?".ToCharArray()) == -1)
        {
            return cardName;
        }
        if (<>f__am$cache1 == null)
        {
            <>f__am$cache1 = c => !char.IsLetterOrDigit(c);
        }
        int index = cardName.IndexOf(cardName.First<char>(<>f__am$cache1));
        string str = cardName.Substring(0, index);
        string str2 = cardName.Substring(index + 1);
        return (str.ToUpper() + '_' + str2);
    }

    private string CleanToAltName(string cardName)
    {
        string villain;
        string str2;
        cardName = this.CleanName(cardName);
        if (<>f__am$cache3 == null)
        {
            <>f__am$cache3 = c => !char.IsLetterOrDigit(c);
        }
        int index = cardName.IndexOf(cardName.FirstOrDefault<char>(<>f__am$cache3));
        if (index < 0)
        {
            villain = cardName.Substring(0, Mathf.Min(2, cardName.Length));
            str2 = cardName.Substring(Mathf.Min(2, cardName.Length));
            index = 2;
        }
        else
        {
            villain = cardName.Substring(0, index - 2);
            str2 = cardName.Substring(index + 1);
        }
        string key = villain;
        if (key != null)
        {
            int num2;
            if (<>f__switch$map0 == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>(10) {
                    { 
                        "BL",
                        0
                    },
                    { 
                        "IT",
                        1
                    },
                    { 
                        "VL",
                        2
                    },
                    { 
                        "Vl",
                        2
                    },
                    { 
                        "vL",
                        2
                    },
                    { 
                        "vl",
                        2
                    },
                    { 
                        "HE",
                        3
                    },
                    { 
                        "hE",
                        3
                    },
                    { 
                        "He",
                        3
                    },
                    { 
                        "he",
                        3
                    }
                };
                <>f__switch$map0 = dictionary;
            }
            if (<>f__switch$map0.TryGetValue(key, out num2))
            {
                switch (num2)
                {
                    case 0:
                        villain = cardName.Substring(0, index);
                        if (!str2.Equals("Gods"))
                        {
                            return (villain + "_BlessingOf" + cardName.Substring(index + 1));
                        }
                        return (villain + "_BlessingOfThe" + cardName.Substring(index + 1));

                    case 1:
                        return (cardName.Substring(0, index) + "_PotionOf" + cardName.Substring(index + 1));

                    case 2:
                        if ((str2.Length <= 3) && !string.IsNullOrEmpty(Scenario.Current.Villain))
                        {
                            villain = Scenario.Current.Villain;
                        }
                        return villain;

                    case 3:
                        if ((str2.Length <= 3) && (Scenario.Current.Henchmen.Length > 0))
                        {
                            villain = Scenario.Current.Henchmen[Scenario.Current.Henchmen.Length - 1];
                        }
                        return villain;
                }
            }
        }
        return villain;
    }

    private string[] GetMatchingCards(string prefix)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < CardTable.Count; i++)
        {
            CardTableEntry entry = CardTable.Get(i);
            if (entry.id.StartsWith(prefix))
            {
                list.Add(entry.id);
            }
        }
        return list.ToArray();
    }

    public override string Run(string[] args)
    {
        if (args.Length < 3)
        {
            return this.HelpText;
        }
        if ((args[1] == "collection") || (args[1] == "col"))
        {
            Card card = CardTable.Create(args[2]);
            if (card != null)
            {
                string displayName = card.DisplayName;
                bool flag = Collection.Add(args[2]);
                card.Destroy();
                if (flag)
                {
                    return base.Success(base.Parameter(displayName) + " added to " + base.Parameter("Collection"));
                }
            }
            return base.Error(args[2] + " not added to " + base.Parameter("Collection"));
        }
        if ((Location.Current == null) || (Turn.Character == null))
        {
            return base.Error("location not loaded");
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window == null)
        {
            return base.Error("window not loaded");
        }
        string[] textArray1 = new string[] { "hand", "discard", "deck", "location", "loc", "blessing", "bless", "collection", "col" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        List<Card> list = new List<Card>();
        if (args[2].IndexOfAny(this.regexAny) >= 0)
        {
            string prefix = args[2].TrimEnd(this.regexAny);
            foreach (string str3 in this.GetMatchingCards(prefix))
            {
                list.Add(CardTable.Create(str3));
            }
            if (list.Count <= 0)
            {
                return base.Error("no matches for " + base.Parameter(args[2]));
            }
        }
        else
        {
            Card item = CardTable.Create(this.TranslateCardName(args[2]));
            if (item == null)
            {
                item = CardTable.Create(this.CleanToAltName(args[2]));
            }
            if (item == null)
            {
                return base.Error("strange card " + base.Parameter(args[2]));
            }
            list.Add(item);
        }
        if (args.Length >= 4)
        {
            int result = 1;
            int.TryParse(args[3], out result);
            result = Mathf.Max(1, result);
            for (int i = 0; i < (result - 1); i++)
            {
                list.Add(CardTable.Create(list[0].ID));
            }
        }
        if (args[1] == "hand")
        {
            foreach (Card card3 in list)
            {
                Turn.Character.Hand.Add(card3, DeckPositionType.Top);
            }
            window.layoutHand.Refresh();
            if (list.Count == 1)
            {
                return base.Success(base.Parameter(list[0].DisplayName) + " added to " + base.Parameter(Turn.Character.DisplayName) + "'s hand");
            }
            return base.Success(base.Parameter(list.Count.ToString()) + " cards added to " + base.Parameter(Turn.Character.DisplayName) + "'s hand");
        }
        if (args[1] == "discard")
        {
            foreach (Card card4 in list)
            {
                Turn.Character.Discard.Add(card4, DeckPositionType.Top);
            }
            window.layoutDiscard.Refresh();
            if (list.Count == 1)
            {
                return base.Success(base.Parameter(list[0].DisplayName) + " added to " + base.Parameter(Turn.Character.DisplayName) + "'s discard");
            }
            return base.Success(base.Parameter(list.Count.ToString()) + " cards added to " + base.Parameter(Turn.Character.DisplayName) + "'s discard");
        }
        if (args[1] == "deck")
        {
            foreach (Card card5 in list)
            {
                Turn.Character.Deck.Add(card5, DeckPositionType.Top);
            }
            window.Refresh();
            if (list.Count == 1)
            {
                return base.Success(base.Parameter(list[0].DisplayName) + " added to " + base.Parameter(Turn.Character.DisplayName) + "'s deck");
            }
            return base.Success(base.Parameter(list.Count.ToString()) + " cards added to " + base.Parameter(Turn.Character.DisplayName) + "'s deck");
        }
        if ((args[1] == "location") || (args[1] == "loc"))
        {
            if (Location.Current.Deck.Count > 0)
            {
                Location.Current.Deck[0].Show(false);
            }
            foreach (Card card6 in list)
            {
                Location.Current.Deck.Add(card6, DeckPositionType.Top);
            }
            window.locationPanel.RefreshCardList();
            window.layoutExplore.Refresh();
            window.layoutExplore.Display();
            if (list.Count == 1)
            {
                return base.Success(base.Parameter(list[0].DisplayName) + " added to " + base.Parameter(Location.Current.DisplayName));
            }
            return base.Success(base.Parameter(list.Count.ToString()) + " cards added to " + base.Parameter(Location.Current.DisplayName));
        }
        if ((args[1] != "blessing") && (args[1] != "bless"))
        {
            return this.HelpText;
        }
        foreach (Card card7 in list)
        {
            Scenario.Current.Blessings.Add(card7, DeckPositionType.Top);
        }
        window.blessingsPanel.Refresh();
        if (list.Count == 1)
        {
            return base.Success(base.Parameter(list[0].DisplayName) + " added to " + base.Parameter("Blessing") + " deck");
        }
        return base.Success(base.Parameter(list.Count.ToString()) + " cards added to " + base.Parameter("Blessing") + " deck");
    }

    private string TranslateCardName(string cardName)
    {
        if ((cardName == "he") || (cardName == "henchman"))
        {
            return Scenario.Current.Henchmen[Scenario.Current.Henchmen.Length - 1];
        }
        if ((cardName == "vl") || (cardName == "villain"))
        {
            return Scenario.Current.Villain;
        }
        if ((cardName == "bl") || (cardName == "blessing"))
        {
            return "BL1B_BlessingOfTheGods";
        }
        if ((cardName == "mayor") || (cardName == "may"))
        {
            return "AL1B_MayorKendraDeverin";
        }
        if (cardName == "mo")
        {
            return "MO1B_Mercenary";
        }
        if (cardName == "al")
        {
            return "AL1B_Soldier";
        }
        if (cardName == "wp")
        {
            return "WP1B_Sling";
        }
        if (cardName == "ar")
        {
            return "AR1B_ChainMail";
        }
        if (cardName == "sp")
        {
            return "SP1B_LightningTouch";
        }
        if (cardName == "bx")
        {
            return "BX1B_TrappedPassageway";
        }
        if (cardName == "it")
        {
            return "IT1B_Caltrops";
        }
        return this.CleanName(cardName);
    }

    public override string Command =>
        "add";

    public override string HelpText =>
        "Syntax: add [hand|discard|deck|location|blessing|collection] id optional:amount";
}

