using System;
using System.Text;

public class DebugCommandList : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 2)
        {
            return this.HelpText;
        }
        if ((args[1] == "collection") || (args[1] == "col"))
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in Collection.Cards)
            {
                builder.Append(str);
                builder.Append(" ");
            }
            return builder.ToString();
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
        string[] textArray1 = new string[] { "hand", "discard", "recharge", "bury", "deck", "location", "loc", "blessing", "bless", "collection", "col" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        Deck hand = null;
        if (args[1] == "hand")
        {
            hand = Turn.Character.Hand;
        }
        if (args[1] == "discard")
        {
            hand = Turn.Character.Discard;
        }
        if (args[1] == "recharge")
        {
            hand = Turn.Character.Recharge;
        }
        if (args[1] == "bury")
        {
            hand = Turn.Character.Bury;
        }
        if (args[1] == "deck")
        {
            hand = Turn.Character.Deck;
        }
        if ((args[1] == "location") || (args[1] == "loc"))
        {
            hand = Location.Current.Deck;
        }
        if ((args[1] == "blessing") || (args[1] == "bless"))
        {
            hand = Scenario.Current.Blessings;
        }
        if (hand == null)
        {
            return this.HelpText;
        }
        StringBuilder builder2 = new StringBuilder();
        for (int i = 0; i < hand.Count; i++)
        {
            builder2.Append(hand[i].ID);
            builder2.Append(" ");
        }
        return builder2.ToString();
    }

    public override string Command =>
        "list";

    public override string HelpText =>
        "Syntax: list [hand|discard|recharge|bury|deck|location|blessing|collection]";
}

