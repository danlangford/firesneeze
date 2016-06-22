using System;

public class DebugCommandProfile : DebugCommand
{
    public override string Run(string[] args)
    {
        if (args.Length < 3)
        {
            return this.HelpText;
        }
        string[] textArray1 = new string[] { "run", "list" };
        if (!base.IsArgValid(args[1], textArray1))
        {
            return base.Error("strange parameter " + base.Parameter(args[1]));
        }
        if (args[1] != "run")
        {
            return this.HelpText;
        }
        DebugProfile profile = new DebugProfile();
        if (profile.Load(args[2]))
        {
            Game.UI.OptionsPanel.Show(false);
            profile.Run();
            return base.Success(base.Parameter(args[1]) + " is running");
        }
        return base.Error(base.Parameter(args[1]) + " is not valid");
    }

    public override string Command =>
        "profile";

    public override string HelpText =>
        "Syntax: profile [run] [name]";
}

