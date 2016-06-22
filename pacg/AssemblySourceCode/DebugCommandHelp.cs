using System;

public class DebugCommandHelp : DebugCommand
{
    public override string Run(string[] args) => 
        DebugParser.HelpText();

    public override string Command =>
        "help";

    public override string HelpText =>
        "Syntax: help";
}

