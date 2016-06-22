using System;

public class DebugCommandMode : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length != 2)
        {
            return this.HelpText;
        }
        if (((args[1] != "god") && (args[1] != "peon")) && (args[1] != "normal"))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        Settings.Debug.GodMode = false;
        Settings.Debug.PeonMode = false;
        if (args[1] == "god")
        {
            Settings.Debug.GodMode = true;
        }
        if (args[1] == "peon")
        {
            Settings.Debug.PeonMode = true;
        }
        return base.Success("mode is " + base.Parameter(args[1]));
    }

    public override string Command =>
        "mode";

    public override string HelpText =>
        "Syntax: mode [god|peon|normal]";
}

