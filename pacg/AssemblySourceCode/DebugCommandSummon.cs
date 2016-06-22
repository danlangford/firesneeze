using System;

public class DebugCommandSummon : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 1)
        {
            return this.HelpText;
        }
        if (args[1] == "clear")
        {
            Settings.Debug.Summons = null;
        }
        else
        {
            Settings.Debug.Summons = args[1];
        }
        return base.Success("summon " + base.Parameter(args[1]));
    }

    public override string Command =>
        "summon";

    public override string HelpText =>
        "Syntax: summon [id|clear]";
}

