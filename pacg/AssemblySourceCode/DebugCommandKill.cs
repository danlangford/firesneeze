using System;

public class DebugCommandKill : DebugCommand
{
    public override string Run(string[] args)
    {
        if ((Party.Characters == null) || (Party.Characters.Count <= 0))
        {
            return base.Error("No party found");
        }
        if ((Turn.Character == null) || (Location.Current == null))
        {
            return base.Error("location not loaded");
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window == null)
        {
            return base.Error("window not loaded");
        }
        Turn.Character.Discard.Combine(Turn.Character.Hand);
        Turn.Character.Discard.Combine(Turn.Character.Deck);
        window.Refresh();
        return base.Success(base.Parameter(Turn.Character.DisplayName) + " is dead");
    }

    public override string Command =>
        "kill";

    public override string HelpText =>
        "Syntax: kill";
}

