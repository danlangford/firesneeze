using System;

public class DebugCommandClose : DebugCommand
{
    public override bool Parse(string command) => 
        (base.Parse(command) || (command == "loc"));

    public override string Run(string[] args)
    {
        if (args.Length < 2)
        {
            return this.HelpText;
        }
        if ((Turn.Character == null) || (Location.Current == null))
        {
            return base.Error("location not loaded");
        }
        args[1] = args[1].ToLower();
        string[] textArray1 = new string[] { "close", "clos", "cl" };
        if (base.IsArgValid(args[1], textArray1))
        {
            CloseType closeType = Turn.CloseType;
            Turn.CloseType = CloseType.Permanent;
            Location.Current.Closed = true;
            Scenario.Current.CloseLocation(Location.Current.ID, CloseType.Permanent);
            if (!Location.Current.Closed)
            {
                Location.Current.Deck.Clear();
                Location.Current.Closed = true;
                Scenario.Current.CloseLocation(Location.Current.ID, CloseType.Permanent);
            }
            Turn.CloseType = closeType;
            return (Location.Current.DisplayName + " closed forcibly.");
        }
        string[] textArray2 = new string[] { "op", "open" };
        if (base.IsArgValid(args[1], textArray2))
        {
            Location.Current.Closed = false;
            Scenario.Current.CloseLocation(Location.Current.ID, CloseType.None);
            UI.Window.Refresh();
            return (Location.Current.DisplayName + " is now open for business.");
        }
        string[] textArray3 = new string[] { "shuffle" };
        if (base.IsArgValid(args[1], textArray3))
        {
            Location.Current.Deck.Shuffle();
            return (Location.Current.DisplayName + " is now shuffled.");
        }
        string[] textArray4 = new string[] { "nat", "na", "nathan", "nath", "vilt", "villaint", "viltop" };
        if (base.IsArgValid(args[1], textArray4))
        {
            Location.Current.Deck.Shuffle();
            for (int j = 0; j < Location.Current.Deck.Count; j++)
            {
                if ((Location.Current.Deck[j].Type == CardType.Villain) || (Location.Current.Deck[j].Type == CardType.Henchman))
                {
                    Card card = Location.Current.Deck[j];
                    Location.Current.Deck.Remove(card);
                    Location.Current.Deck.Add(card, DeckPositionType.Top);
                    UI.Window.Refresh();
                    return (Location.Current.DisplayName + " much shuffled. Nathan guarentee.");
                }
            }
            return base.Error("Could not find a henchman/villain to put on top. Just shuffled instead.");
        }
        string[] textArray5 = new string[] { "doom", "doo", "vilb", "villainb", "vilbot" };
        if (!base.IsArgValid(args[1], textArray5))
        {
            return base.Error("could not recognize command " + args[1]);
        }
        Location.Current.Deck.Shuffle();
        for (int i = 0; i < Location.Current.Deck.Count; i++)
        {
            if ((Location.Current.Deck[i].Type == CardType.Villain) || (Location.Current.Deck[i].Type == CardType.Henchman))
            {
                Card card2 = Location.Current.Deck[i];
                Location.Current.Deck.Remove(card2);
                Location.Current.Deck.Add(card2, DeckPositionType.Bottom);
                UI.Window.Refresh();
                return (Location.Current.DisplayName + " shuffled to ensure your doom");
            }
        }
        return base.Error("Location doesn't have henchman/villain anyways. Just shuffled instead.");
    }

    public override string Command =>
        "location";

    public override string HelpText =>
        "Syntax: location [open|close|shuffle|viltop|vilbot]";
}

