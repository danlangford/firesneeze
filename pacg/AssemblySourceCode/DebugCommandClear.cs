using System;

public class DebugCommandClear : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 2)
        {
            return this.HelpText;
        }
        if ((args[1] == "collection") || (args[1] == "col"))
        {
            Collection.Clear();
            return base.Success(base.Parameter("Collection") + " was cleared locally");
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
        if (args[1] == "hand")
        {
            Turn.Character.Hand.Clear();
            window.layoutHand.Refresh();
            return base.Success(base.Parameter(Turn.Character.DisplayName) + "'s hand was cleared");
        }
        if (args[1] == "deck")
        {
            Turn.Character.Deck.Clear();
            window.Refresh();
            return base.Success(base.Parameter(Turn.Character.DisplayName) + "'s deck was cleared");
        }
        if (args[1] == "discard")
        {
            Turn.Character.Discard.Clear();
            window.layoutDiscard.Refresh();
            return base.Success(base.Parameter(Turn.Character.DisplayName) + "'s discard was cleared");
        }
        if ((args[1] == "location") || (args[1] == "loc"))
        {
            Location.Current.Deck.Clear();
            window.locationPanel.RefreshCardList();
            window.layoutExplore.Refresh();
            window.layoutExplore.Display();
            return base.Success(base.Parameter(Location.Current.DisplayName) + " was cleared");
        }
        if ((args[1] != "blessing") && (args[1] != "bless"))
        {
            return this.HelpText;
        }
        Scenario.Current.Blessings.Clear();
        window.blessingsPanel.Refresh();
        return base.Success(base.Parameter("Blessing") + " deck was cleared");
    }

    public override string Command =>
        "clear";

    public override string HelpText =>
        "Syntax: clear [hand|discard|deck|location|blessing|collection]";
}

