using System;
using System.Collections.Generic;
using System.Text;

public class DebugParser
{
    private static List<DebugCommand> commands;
    private static char[] SEPERATOR = new char[] { ' ' };

    static DebugParser()
    {
        List<DebugCommand> list = new List<DebugCommand> {
            new DebugCommandHelp(),
            new DebugCommandMode(),
            new DebugCommandAdd(),
            new DebugCommandClear(),
            new DebugCommandList(),
            new DebugCommandScenario(),
            new DebugCommandSummon(),
            new DebugCommandLicense(),
            new DebugCommandLevelup(),
            new DebugCommandProfile(),
            new DebugCommandReport(),
            new DebugCommandClose(),
            new DebugCommandSave(),
            new DebugCommandVersion(),
            new DebugCommandBox(),
            new DebugCommandSpeed(),
            new DebugCommandInfo(),
            new DebugCommandCrookedDice(),
            new DebugCommandExperience(),
            new DebugCommandVault(),
            new DebugCommandDirectory(),
            new DebugCommandTutorial(),
            new DebugCommandGold(),
            new DebugCommandPhase(),
            new DebugCommandKill()
        };
        commands = list;
    }

    public static string Execute(string command)
    {
        string[] args = command.Split(SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
        if (args.Length > 0)
        {
            args[0] = args[0].ToLower();
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Parse(args[0]))
                {
                    return commands[i].Run(args);
                }
            }
        }
        return null;
    }

    public static string HelpText()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < commands.Count; i++)
        {
            builder.Append(commands[i].Command);
            builder.Append(" ");
        }
        return builder.ToString();
    }
}

