using System;

public class DebugCommandExperience : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 2)
        {
            return this.HelpText;
        }
        if ((Turn.Character == null) || (Location.Current == null))
        {
            return base.Error("character not loaded");
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window == null)
        {
            return base.Error("window not loaded");
        }
        int result = 0;
        int.TryParse(args[1], out result);
        if (result > 0)
        {
            Character character = Turn.Character;
            character.XPX += result;
            return base.Success(base.Parameter(Turn.Character.DisplayName) + " gained " + base.Parameter(result.ToString()));
        }
        return base.Error("invalid value: " + base.Parameter(args[1]));
    }

    public override string Command =>
        "xp";

    public override string HelpText =>
        "Syntax: xp [amount]";
}

