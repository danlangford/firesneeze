using System;

public class DebugCommandBox : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length != 2)
        {
            return this.HelpText;
        }
        if ((args[1] != "list") && (args[1] != "dump"))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        bool verbose = false;
        if (args[1] == "dump")
        {
            verbose = true;
        }
        string s = Campaign.Box.Dump("DEBUG", verbose);
        return base.Success(s);
    }

    public override string Command =>
        "box";

    public override string HelpText =>
        "Syntax: box [list|dump]";
}

