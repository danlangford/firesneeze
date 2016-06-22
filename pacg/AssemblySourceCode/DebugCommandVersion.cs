using System;

public class DebugCommandVersion : DebugCommand
{
    public override string Run(string[] args) => 
        base.Success("version is " + base.Parameter(Game.Instance.BuildNumber));

    public override string Command =>
        "version";

    public override string HelpText =>
        "Syntax: version";
}

